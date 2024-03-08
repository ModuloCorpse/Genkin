using System.Text;

namespace Ginko
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

        public List<Transaction> GetTransactionsBetweenDates(DateTime startDate, DateTime endDate)
        {
            List<Transaction> result = [];
            int startIndex = m_Transactions.FindFirstElementAfterOrOnDate(startDate);
            for (int i = startIndex; i < m_Transactions.Count && m_Transactions[i].Date <= endDate; i++)
                result.Add(m_Transactions[i]);
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

        public override string ToString()
        {
            StringBuilder builder = new("[Month: ");
            builder.Append(string.Format("Date = {0}, Real amount = {1}, Amount = {2}, Transactions = [", m_Date, m_RealAmount, m_Amount));
            if (m_Transactions.Count > 0)
                builder.AppendLine();
            foreach (Transaction transaction in m_Transactions)
                builder.AppendLine(transaction.ToString());
            builder.Append("], Holdings = [");
            if (m_Holdings.Count > 0)
                builder.AppendLine();
            foreach (Transaction holding in m_Holdings)
                builder.AppendLine(holding.ToString());
            builder.Append("]]");
            return builder.ToString();
        }
    }
}
