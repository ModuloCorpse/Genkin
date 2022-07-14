namespace Ginko
{
    public class TransactionStorage
    {
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, List<Transaction>>>> m_Transactions = new();

        public void AddTransaction(Transaction transaction)
        {
            DateTime transactionTime = transaction.GetTime();
            if (!m_Transactions.ContainsKey(transactionTime.Year))
                m_Transactions[transactionTime.Year] = new();
            Dictionary<int, Dictionary<int, List<Transaction>>> transactionOfYear = m_Transactions[transactionTime.Year];
            if (!transactionOfYear.ContainsKey(transactionTime.Month))
                transactionOfYear[transactionTime.Month] = new();
            Dictionary<int, List<Transaction>> transactionOfMonth = transactionOfYear[transactionTime.Month];
            if (!transactionOfMonth.ContainsKey(transactionTime.Day))
                transactionOfMonth[transactionTime.Day] = new();
            transactionOfMonth[transactionTime.Day].Add(transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            DateTime transactionTime = transaction.GetTime();
            if (!m_Transactions.ContainsKey(transactionTime.Year))
                return;
            Dictionary<int, Dictionary<int, List<Transaction>>> transactionOfYear = m_Transactions[transactionTime.Year];
            if (!transactionOfYear.ContainsKey(transactionTime.Month))
                return;
            Dictionary<int, List<Transaction>> transactionOfMonth = transactionOfYear[transactionTime.Month];
            if (!transactionOfMonth.ContainsKey(transactionTime.Day))
                return;
            transactionOfMonth[transactionTime.Day].RemoveAll(elem => elem == transaction);
        }

        public static bool IsMonthOfYearBetween(int month, int year, DateTime begin, DateTime end)
        {
            return ( //We check if date is NOT between begin and end and return the opposite
                (year > begin.Year && year < end.Year) || //Year is strictly between begin and end
                (year == begin.Year && year == end.Year && month >= begin.Month && month <= end.Month) || //Month is strictly between begin and end
                (year == begin.Year && year != end.Year && month >= begin.Month) || //Month is after begin
                (year == end.Year && year != begin.Year && month <= end.Month)); //Year is the same as end but month is after
        }

        public List<Transaction> GetAllTransactionsBetween(DateTime begin, DateTime end)
        {
            List<Transaction> transactionList = new();
            foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, List<Transaction>>>> transactionOfYear in m_Transactions)
            {
                int year = transactionOfYear.Key;
                if (year >= begin.Year && year <= end.Year)
                {
                    foreach (KeyValuePair<int, Dictionary<int, List<Transaction>>> transactionOfMonth in transactionOfYear.Value)
                    {
                        int month = transactionOfMonth.Key;
                        if ((year > begin.Year && year < end.Year) ||
                            (year == begin.Year && year == end.Year && month >= begin.Month && month <= end.Month) ||
                            (year == begin.Year && year != end.Year && month >= begin.Month) ||
                            (year == end.Year && year != begin.Year && month <= end.Month))
                        {
                            foreach (KeyValuePair<int, List<Transaction>> transactionOfDay in transactionOfMonth.Value)
                            {
                                DateTime transactionsTime = new(year, month, transactionOfDay.Key);
                                if (DateTime.Compare(transactionsTime, begin) >= 0 && DateTime.Compare(transactionsTime, end) <= 0)
                                    transactionList.AddRange(transactionOfDay.Value);
                            }
                        }
                    }
                }
            }
            return transactionList;
        }

        public List<Transaction> GetAllTransactions()
        {
            List<Transaction> transactionList = new();
            foreach (Dictionary<int, Dictionary<int, List<Transaction>>> transactionOfYear in m_Transactions.Values)
            {
                foreach (Dictionary<int, List<Transaction>> transactionOfMonth in transactionOfYear.Values)
                {
                    foreach (List<Transaction> transactionOfDay in transactionOfMonth.Values)
                    {
                        transactionList.AddRange(transactionOfDay);
                    }
                }
            }
            return transactionList;
        }

        public List<Transaction> GetAllTransactionsOfYear(int year)
        {
            return GetAllTransactionsBetween(new(year, 1, 1), new(year, 12, 31));
        }

        public List<Transaction> GetAllTransactionsOfMonth(int year, int month)
        {
            DateTime firstDayOfMonth = new(year, month, 1);
            return GetAllTransactionsBetween(firstDayOfMonth, firstDayOfMonth.AddMonths(1).AddDays(-1));
        }

        public List<Transaction> GetAllTransactionsOfTime(DateTime time)
        {
            if (!m_Transactions.ContainsKey(time.Year))
            {
                Dictionary<int, Dictionary<int, List<Transaction>>> transactionOfYear = m_Transactions[time.Year];
                if (!transactionOfYear.ContainsKey(time.Month))
                {
                    Dictionary<int, List<Transaction>> transactionOfMonth = transactionOfYear[time.Month];
                    if (transactionOfMonth.ContainsKey(time.Day))
                        return transactionOfMonth[time.Day];
                }
            }
            return new();
        }
    }
}
