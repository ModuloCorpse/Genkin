using CorpseLib.Serialize;
using CorpseLib;
using System.Text;
using System.Collections;

namespace Ginko
{
    public class Account(string name, string description, decimal amount, Guid id) : FinancialItem(name, description, amount, id), IEnumerable<Saving>
    {
        public class BytesSerializer : BytesSerializer<Account>
        {
            protected override OperationResult<Account> DeserializeItem(BytesReader reader, Guid id, decimal amount, string name, string description)
            {
                OperationResult<Saving> defaultSaving = reader.Read<Saving>();
                if (!defaultSaving)
                    return new(defaultSaving.Error, defaultSaving.Description);
                Account account;
                if (defaultSaving.Result != null)
                    account = new(name, description, amount, id, defaultSaving.Result);
                else
                    account = new(name, description, amount, id);
                int savingCount = reader.ReadInt();
                for (int i = 0; i != savingCount; i++)
                {
                    OperationResult<Saving> saving = reader.Read<Saving>();
                    if (saving && saving.Result != null)
                        account.AddSaving(saving.Result);
                }
                return new(account);
            }

            protected override void SerializeItem(Account obj, BytesWriter writer)
            {
                writer.Write(obj.m_DefaultSaving);
                writer.Write(obj.m_Savings.Count);
                foreach (Saving saving in obj.m_Savings)
                    writer.Write(saving);
            }
        }

        private class SavingsEnumerator(Account account) : IEnumerator<Saving>
        {
            private readonly Account m_Account = account;
            private int m_Position = -1;

            public bool MoveNext()
            {
                m_Position++;
                return (m_Position < m_Account.Count);
            }

            public void Reset() => m_Position = -1;

            public void Dispose() { }

            object IEnumerator.Current { get => Current; }

            public Saving Current
            {
                get
                {
                    try
                    {
                        return m_Account[m_Position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private readonly Saving m_DefaultSaving = new(name, description, amount, id);
        private readonly List<Saving> m_Savings = [];

        public int Count => m_Savings.Count + 1;
        public Saving this[int idx] { get => (idx == 0) ? m_DefaultSaving : m_Savings[idx - 1]; }

        private Account(string name, string description, decimal amount, Guid id, Saving defaultSaving) : this(name, description, amount, id) => m_DefaultSaving = defaultSaving;

        private void AddSaving(Saving saving) => m_Savings.Add(saving);

        public override string ToString()
        {
            StringBuilder builder = new("[Account: ");
            builder.Append(base.ToString());
            builder.Append("Savings = [");
            builder.AppendLine();
            builder.AppendLine(m_DefaultSaving.ToString());
            foreach (Saving saving in m_Savings)
                builder.AppendLine(saving.ToString());
            builder.Append("]]");
            return builder.ToString();
        }

        public IEnumerator<Saving> GetEnumerator() => new SavingsEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
