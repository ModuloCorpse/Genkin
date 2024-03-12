using CorpseLib.Serialize;
using CorpseLib;
using System.Text;

namespace Genkin.Core
{
    public class Saving(string name, string description, decimal amount, Guid id)
    {
        public class BytesSerializer : ABytesSerializer<Saving>
        {
            protected override OperationResult<Saving> Deserialize(ABytesReader reader)
            {
                Guid id = reader.Read<Guid>();
                decimal amount = reader.Read<decimal>();
                string name = reader.Read<string>();
                string description = reader.Read<string>();
                Saving saving = new(name, description, amount, id);
                OperationResult<List<Transaction>> transactionsResult = reader.ReadList<Transaction>();
                if (!transactionsResult)
                    return new(transactionsResult.Error, transactionsResult.Description);
                List<Transaction> transactions = transactionsResult.Result!;
                foreach (Transaction transaction in transactions)
                    saving.AddTransaction(transaction);
                OperationResult<List<Transaction>> holdingsResult = reader.ReadList<Transaction>();
                if (!holdingsResult)
                    return new(holdingsResult.Error, holdingsResult.Description);
                List<Transaction> holdings = holdingsResult.Result!;
                foreach (Transaction holding in holdings)
                    saving.AddHolding(holding);
                return new(saving);
            }

            protected override void Serialize(Saving obj, ABytesWriter writer)
            {
                writer.Write(obj.m_ID);
                writer.Write(obj.m_Amount.Value);
                writer.Write(obj.m_Name);
                writer.Write(obj.m_Description);
                List<Transaction> transactions = [];
                List<Transaction> holdings = [];
                foreach (FinancialMonth month in obj.m_Months)
                {
                    transactions.AddRange(month.Transactions);
                    holdings.AddRange(month.Holdings);
                }
                writer.WriteList(transactions);
                writer.WriteList(holdings);
            }
        }

        private readonly Timeline<FinancialMonth> m_Months = [];
        protected Amount m_Amount = new(amount);
        protected readonly Guid m_ID = id;
        protected readonly string m_Name = name;
        protected readonly string m_Description = description;

        public Guid ID => m_ID;
        public string Name => m_Name;
        public string Description => m_Description;
        public string Currency => m_Amount.Currency;
        public decimal Amount => m_Amount.Value;

        internal void UpdateCurrency(string currency)
        {
            m_Amount = new(currency, m_Amount.Value);
            foreach (FinancialMonth month in m_Months)
                month.UpdateCurrency(currency);
        }

        private int GetMonthOfTransaction(Transaction transaction)
        {
            DateTime month = new(transaction.Date.Year, transaction.Date.Month, 1);
            if (m_Months.Count == 0)
            {
                FinancialMonth newMonth = new(month, Amount);
                m_Months.Add(newMonth);
                return 0;
            }
            int index = m_Months.FindFirstElementAfterOrOnDate(month);
            if (index == 0)
            {
                if (month == m_Months[0].Date)
                    return 0;
                DateTime current = m_Months[0].Date.AddMonths(-1);
                while (current >= month)
                {
                    FinancialMonth newMonth = new(current, Amount);
                    newMonth.SetNextMonth(m_Months[0]);
                    m_Months.Add(newMonth);
                    current = current.AddMonths(-1);
                }
                return 0;
            }
            else if (index == m_Months.Count)
            {
                DateTime current = m_Months[^1].Date.AddMonths(1);
                while (current <= month)
                {
                    FinancialMonth newMonth = new(current, Amount);
                    m_Months[^1].SetNextMonth(newMonth);
                    m_Months.Add(newMonth);
                    current = current.AddMonths(1);
                }
                return m_Months.Count - 1;
            }
            else
                return index;
        }

        internal void AddTransaction(Transaction transaction)
        {
            transaction.SetCurrency(Currency);
            int index = GetMonthOfTransaction(transaction);
            if (index < m_Months.Count)
                m_Months[index].AddTransaction(transaction);
        }

        public void NewTransaction(string name, string description, DateTime date, decimal amount)
        {
            AddTransaction(new(name, description, date, amount, Guid.NewGuid()));
        }

        public void AddHolding(Transaction holding)
        {
            holding.SetCurrency(Currency);
            int index = GetMonthOfTransaction(holding);
            if (index < m_Months.Count)
                m_Months[index].AddHolding(holding);
        }
        public void NewHolding(string name, string description, DateTime date, decimal amount)
        {
            AddHolding(new(name, description, date, amount, Guid.NewGuid()));
        }

        public void RemoveTransaction(Transaction transaction)
        {
            DateTime month = new(transaction.Date.Year, transaction.Date.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(month);
            if (index < m_Months.Count)
                m_Months[index].RemoveTransaction(transaction);
        }

        public void RemoveHolding(Transaction transaction)
        {
            DateTime month = new(transaction.Date.Year, transaction.Date.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(month);
            if (index < m_Months.Count)
                m_Months[index].RemoveHolding(transaction);
        }

        public List<Transaction> GetTransactions(Filter filter)
        {
            List<Transaction> result = [];
            DateTime firstMonth = new(filter.StartDate.Year, filter.StartDate.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(firstMonth);
            while (index < m_Months.Count && m_Months[index].Date < filter.EndDate)
                result.AddRange(m_Months[index].GetTransactions(filter));
            return result;
        }

        public decimal GetAmountAt(DateTime date)
        {
            DateTime month = new(date.Year, date.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(month);
            if (index < m_Months.Count)
                return m_Months[index].GetAmountAt(date);
            else
                return m_Months[^1].GetAmountAt(date);
        }

        public decimal GetRealAmountAt(DateTime date)
        {
            DateTime month = new(date.Year, date.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(month);
            if (index < m_Months.Count)
                return m_Months[index].GetRealAmountAt(date);
            else
                return m_Months[^1].GetRealAmountAt(date);
        }

        public string ToString(int indent)
        {
            string indentStr = new(' ', indent);
            StringBuilder builder = new(indentStr);
            builder.Append($"[Saving: ID = {m_ID}, Amount = {m_Amount}, Name = \"{m_Name}\", Description = \"{m_Description}\", Months = [");
            if (m_Months.Count > 0)
            {
                builder.AppendLine();
                builder.Append(indentStr);
            }
            foreach (FinancialMonth month in m_Months)
            {
                builder.AppendLine(month.ToString(indent + 1));
                builder.Append(indentStr);
            }
            builder.Append("]]");
            return builder.ToString();
        }

        public override string ToString() => ToString(0);
    }
}
