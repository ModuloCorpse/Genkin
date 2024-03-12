using System;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace Genkin.Core
{
    public class FinancialMonth(DateTime date, decimal amount) : ITimelineElement
    {
        private readonly Timeline<Transaction> m_Transactions = [];
        private readonly List<Transaction> m_Holdings = [];
        private FinancialMonth? m_PreviousMonth = null;
        private FinancialMonth? m_NextMonth = null;
        private readonly DateTime m_Date = date;
        private readonly decimal m_OriginalAmmount = amount;
        private decimal m_RealAmount = amount;
        private decimal m_Amount = amount;

        internal Timeline<Transaction> Transactions => m_Transactions;
        internal List<Transaction> Holdings => m_Holdings;
        public DateTime Date => m_Date;

        private void UpdateAmounts()
        {
            m_RealAmount = m_PreviousMonth?.m_RealAmount ?? m_OriginalAmmount;
            m_Amount = m_PreviousMonth?.m_Amount ?? m_OriginalAmmount;
            foreach (Transaction transaction in m_Transactions)
            {
                m_RealAmount += transaction.Amount;
                m_Amount += transaction.Amount;
            }
            foreach (Transaction holding in m_Holdings)
                m_Amount += holding.Amount;
            m_NextMonth?.UpdateAmounts();
        }

        internal void UpdateCurrency(string currency)
        {
            foreach (Transaction transaction in m_Transactions)
                transaction.SetCurrency(currency);
            foreach (Transaction transaction in m_Holdings)
                transaction.SetCurrency(currency);
        }

        internal void SetNextMonth(FinancialMonth nextMonth)
        {
            m_NextMonth = nextMonth;
            m_NextMonth.m_PreviousMonth = this;
            m_NextMonth.UpdateAmounts();
        }

        public void AddTransaction(Transaction transaction)
        {
            m_Transactions.Add(transaction);
            UpdateAmounts();
        }

        public void AddHolding(Transaction holding)
        {
            m_Holdings.Add(holding);
            UpdateAmounts();
        }

        public void RemoveTransaction(Transaction transaction)
        {
            int index = m_Transactions.FindFirstElementAfterOrOnDate(transaction.Date);
            while (m_Transactions[index].Date == transaction.Date)
            {
                if (m_Transactions[index].ID == transaction.ID)
                {
                    m_Transactions.RemoveAt(index);
                    UpdateAmounts();
                    return;
                }
                index++;
            }
        }

        public void RemoveHolding(Transaction transaction)
        {
            int index = 0;
            foreach (Transaction holding in m_Holdings)
            {
                if (holding.ID == transaction.ID)
                {
                    m_Holdings.RemoveAt(index);
                    UpdateAmounts();
                    return;
                }
                index++;
            }
        }

        public List<Transaction> GetTransactions(Filter filter)
        {
            List<Transaction> result = [];
            int startIndex = m_Transactions.FindFirstElementAfterOrOnDate(filter.StartDate);
            for (int i = startIndex; i < m_Transactions.Count && m_Transactions[i].Date <= filter.EndDate; i++)
            {
                if (filter.Match(m_Transactions[i]))
                    result.Add(m_Transactions[i]);
            }
            return result;
        }

        public decimal GetRealAmountAt(DateTime date)
        {
            decimal realAmount = m_PreviousMonth?.m_RealAmount ?? m_OriginalAmmount;
            for (int i = 0; i < m_Transactions.Count && m_Transactions[i].Date < date; i++)
                realAmount += m_Transactions[i].Amount;
            return realAmount;
        }

        public decimal GetAmountAt(DateTime date)
        {
            decimal amount = m_PreviousMonth?.m_Amount ?? m_OriginalAmmount;
            for (int i = 0; i < m_Transactions.Count && m_Transactions[i].Date < date; i++)
                amount += m_Transactions[i].Amount;
            foreach (Transaction holding in m_Holdings)
                amount += holding.Amount;
            return amount;
        }

        public string ToString(int indent)
        {
            string indentStr = new(' ', indent);
            StringBuilder builder = new(indentStr);
            builder.Append("[Month: ");
            builder.Append(string.Format("Date = {0}, Real amount = {1}, Amount = {2}, Transactions = [", m_Date, m_RealAmount, m_Amount));
            if (m_Transactions.Count > 0)
            {
                builder.AppendLine();
                builder.Append(indentStr);
            }
            foreach (Transaction transaction in m_Transactions)
            {
                builder.AppendLine(string.Format("{0}{1}", new string(' ', indent + 1), transaction));
                builder.Append(indentStr);
            }
            builder.Append("], Holdings = [");
            if (m_Holdings.Count > 0)
            {
                builder.AppendLine();
                builder.Append(indentStr);
            }
            foreach (Transaction holding in m_Holdings)
            {
                builder.AppendLine(string.Format("{0}{1}", new string(' ', indent + 1), holding));
                builder.Append(indentStr);
            }
            builder.Append("]]");
            return builder.ToString();
        }

        public override string ToString() => ToString(0);
    }
}
