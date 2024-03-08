using CorpseLib.Serialize;
using CorpseLib;
using System.Text;

namespace Ginko
{
    public class Saving(string name, string description, decimal amount, Guid id) : FinancialItem(name, description, amount, id)
    {
        public class BytesSerializer : BytesSerializer<Saving>
        {
            protected override OperationResult<Saving> DeserializeItem(BytesReader reader, Guid id, decimal amount, string name, string description)
            {
                Saving saving = new(name, description, amount, id);
                int transactionCount = reader.ReadInt();
                for (int i = 0; i != transactionCount; i++)
                {
                    OperationResult<Transaction> transaction = reader.Read<Transaction>();
                    if (transaction && transaction.Result != null)
                        saving.AddTransaction(transaction.Result);
                }
                int holdingCount = reader.ReadInt();
                for (int i = 0; i != holdingCount; i++)
                {
                    OperationResult<Transaction> holding = reader.Read<Transaction>();
                    if (holding && holding.Result != null)
                        saving.AddHolding(holding.Result);
                }
                return new(saving);
            }

            protected override void SerializeItem(Saving obj, BytesWriter writer)
            {
                List<Transaction> transactions = [];
                List<Transaction> holdings = [];
                foreach (FinancialMonth month in obj.m_Months)
                {
                    transactions.AddRange(month.Transactions);
                    holdings.AddRange(month.Holdings);
                }
                writer.Write(transactions.Count);
                foreach (Transaction transaction in transactions)
                    writer.Write(transaction);
                writer.Write(holdings.Count);
                foreach (Transaction holding in holdings)
                    writer.Write(holding);
            }
        }

        private readonly Timeline<FinancialMonth> m_Months = [];

        private int GetMonthOfTransaction(Transaction transaction)
        {
            DateTime month = new(transaction.Date.Year, transaction.Date.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(month);
            if (index == 0)
            {
                //TODO Create all month between the one needed and the first one
                //return 0;
                return m_Months.Count;
            }
            else if (index == m_Months.Count)
            {
                //TODO Create all month between the last one and the one needed
                //return m_Months.Count - 1;
                return m_Months.Count;
            }
            else
                return index;
        }

        public void AddTransaction(Transaction transaction)
        {
            int index = GetMonthOfTransaction(transaction);
            if (index < m_Months.Count)
                m_Months[index].AddTransaction(transaction);
        }

        public void AddHolding(Transaction holding)
        {
            int index = GetMonthOfTransaction(holding);
            if (index < m_Months.Count)
                m_Months[index].AddHolding(holding);
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

        public List<Transaction> GetTransactionsBetweenDates(DateTime startDate, DateTime endDate)
        {
            List<Transaction> result = [];
            DateTime firstMonth = new(startDate.Year, startDate.Month, 1);
            int index = m_Months.FindFirstElementAfterOrOnDate(firstMonth);
            while (index < m_Months.Count && m_Months[index].Date < endDate)
                result.AddRange(m_Months[index].GetTransactionsBetweenDates(startDate, endDate));
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

        public override string ToString()
        {
            StringBuilder builder = new("[Saving: ");
            builder.Append(base.ToString());
            builder.Append("Months = [");
            if (m_Months.Count > 0)
                builder.AppendLine();
            foreach (FinancialMonth month in m_Months)
                builder.AppendLine(month.ToString());
            builder.Append("]]");
            return builder.ToString();
        }
    }
}
