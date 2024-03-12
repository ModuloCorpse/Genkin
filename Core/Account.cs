using CorpseLib.Serialize;
using CorpseLib;
using System.Text;
using System.Collections;

namespace Genkin.Core
{
    public class Account : IEnumerable<Saving>
    {
        public class BytesSerializer : ABytesSerializer<Account>
        {
            protected override OperationResult<Account> Deserialize(ABytesReader reader)
            {
                Guid id = reader.Read<Guid>();
                string name = reader.Read<string>();
                string description = reader.Read<string>();
                string currency = reader.Read<string>();
                OperationResult<List<Saving>> savingsResult = reader.ReadList<Saving>();
                if (!savingsResult)
                    return new(savingsResult.Error, savingsResult.Description);
                List<Saving> savings = savingsResult.Result!;
                Account account = new(name, description, currency, id, savings[0]);
                for (int i = 1; i != savings.Count; i++)
                {
                    Saving saving = savings[i];
                    account.AddSaving(saving);
                }
                return new(account);
            }

            protected override void Serialize(Account obj, ABytesWriter writer)
            {
                writer.Write(obj.m_ID);
                writer.Write(obj.m_Name);
                writer.Write(obj.m_Description);
                writer.Write(obj.m_Currency);
                writer.WriteList(obj.m_Savings.Values);
            }
        }

        private readonly Dictionary<Guid, Saving> m_Savings = [];
        protected readonly Guid m_ID;
        protected readonly string m_Name;
        protected readonly string m_Description;
        protected readonly string m_Currency;

        public Guid ID => m_ID;
        public string Name => m_Name;
        public string Description => m_Description;
        public string Currency => m_Currency;
        public decimal Amount
        {
            get
            {
                decimal amount = 0;
                foreach (var savings in m_Savings.Values)
                    amount += savings.Amount;
                return amount;
            }
        }
        public int Count => m_Savings.Count;
        public Saving this[Guid id] { get => m_Savings[id]; }

        public Account(string name, string description, string currency, Guid id)
        {
            m_Name = name;
            m_Description = description;
            m_Currency = currency;
            m_ID = id;
        }

        private Account(string name, string description, string currency, Guid id, Saving defaultSaving)
        {
            m_Name = name;
            m_Description = description;
            m_Currency = currency;
            m_ID = id;
            AddSaving(defaultSaving);
        }

        private void AddSaving(Saving saving)
        {
            saving.UpdateCurrency(Currency);
            m_Savings[saving.ID] = saving;
        }

        public Saving NewSaving(string name, string description, decimal amount)
        {
            Saving saving = new(name, description, amount, Guid.NewGuid());
            AddSaving(saving);
            return saving;
        }

        public void NewTransfer(Guid fromID, Guid toID, string name, string description, DateTime date, decimal amount)
        {
            if (m_Savings.TryGetValue(fromID, out Saving? from) && m_Savings.TryGetValue(toID, out Saving? to))
            {
                Guid guid = Guid.NewGuid();
                from.AddTransaction(new(name, description, date, -amount, guid));
                to.AddTransaction(new(name, description, date, amount, guid));
            }
        }

        public List<Transaction> GetTransactions(Filter filter)
        {
            List<Transaction> result = [];
            foreach (Saving saving in m_Savings.Values)
            {
                if (filter.Saving.Count == 0 || filter.Saving.Contains(saving.ID))
                    result.AddRange(saving.GetTransactions(filter));
            }
            return result;
        }

        public string ToString(int indent)
        {
            string indentStr = new(' ', indent);
            StringBuilder builder = new(indentStr);
            builder.AppendLine($"[Account: ID = {m_ID}, Amount = {Amount}{m_Currency}, Name = \"{m_Name}\", Description = \"{m_Description}\", Savings = [");
            builder.Append(indentStr);
            foreach (Saving saving in m_Savings.Values)
            {
                builder.AppendLine(saving.ToString(indent + 1));
                builder.Append(indentStr);
            }
            builder.Append("]]");
            return builder.ToString();
        }

        public override string ToString() => ToString(0);

        public IEnumerator<Saving> GetEnumerator() => m_Savings.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
