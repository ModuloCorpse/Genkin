namespace Ginko
{
    public class Account: DatabaseObject
    {
        private readonly Dictionary<DateTime, double> m_Markers = new();
        private readonly TransactionStorage m_Transactions = new();
        private readonly Dictionary<string, Account> m_SubAccounts = new();
        private readonly DateTime m_TimeLimit;

        public Account(Database database, double amount, string name, string description = "", DateTime? timeLimit = null): base(database, amount, name, description)
        {
            m_TimeLimit = (timeLimit != null ? (DateTime)timeLimit : new(1970, 1, 1));
        }

        private double GetTransactionAmount(Transaction transaction)
        {
            if (transaction.GetFrom() == this)
                return -transaction.GetAmount();
            return transaction.GetAmount();
        }

        public void NewTransaction(string name, string descritpion, double ammount, DateTime time, Account to)
        {
            Transaction transaction = m_Database.NewTransaction(ammount, name, descritpion, time, this, to);
            AddTransaction(transaction);
            if (to != null)
                to.AddTransaction(transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (transaction.GetFrom() == this || transaction.GetTo() == this)
                m_Transactions.RemoveTransaction(transaction);
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction.GetFrom() == this || transaction.GetTo() == this)
                m_Transactions.AddTransaction(transaction);
        }

        private double GetMarkerAmount(DateTime markerTime)
        {
            if (!m_Markers.ContainsKey(markerTime))
            {
                if (DateTime.Compare(markerTime, m_TimeLimit) < 1)
                    m_Markers[m_TimeLimit] = GetAmount();
                else
                    GenerateMarkerFor(markerTime);
            }

            return m_Markers[markerTime];
        }

        public double GetAmmountAt(DateTime time)
        {
            DateTime markerTime = new(time.Year, time.Month, 1);
            double amount = GetMarkerAmount(markerTime);
            List<Transaction> transactions = m_Transactions.GetAllTransactionsBetween(markerTime, time);
            foreach (Transaction transaction in transactions)
                amount += GetTransactionAmount(transaction);
            return amount;
        }

        private void RegenerateMarkerFor(DateTime markerTime)
        {
            if (!m_Markers.ContainsKey(markerTime))
            {
                GenerateMarkerFor(markerTime);
                return;
            }

            DateTime previousMarkerTime = markerTime.AddMonths(-1);
            double markerAmount = GetMarkerAmount(previousMarkerTime);
            List<Transaction> transactions = m_Transactions.GetAllTransactionsOfMonth(previousMarkerTime.Year, previousMarkerTime.Month);
            foreach (Transaction transaction in transactions)
                markerAmount += GetTransactionAmount(transaction);
            m_Markers[markerTime] = markerAmount;
            MarkForUpdate();
            DateTime nextMarkerTime = markerTime.AddMonths(1);
            RegenerateMarkerFor(nextMarkerTime);
        }

        private void GenerateMarkerFor(DateTime markerTime)
        {
            if (m_Markers.ContainsKey(markerTime))
                return;
            DateTime previousMarkerTime = markerTime.AddMonths(-1);
            double markerAmount = GetMarkerAmount(previousMarkerTime);
            List<Transaction> transactions = m_Transactions.GetAllTransactionsOfMonth(previousMarkerTime.Year, previousMarkerTime.Month);
            foreach (Transaction transaction in transactions)
                markerAmount += GetTransactionAmount(transaction);
            m_Markers[markerTime] = markerAmount;
            MarkForUpdate();
        }

        public void UpdateMarker(DateTime time)
        {
            DateTime markerTime = new(time.Year, time.Month, 1);
            foreach (Account subAccount in m_SubAccounts.Values)
                subAccount.UpdateMarker(markerTime);
            RegenerateMarkerFor(markerTime);
        }

        public void CreateMarker(DateTime time)
        {
            DateTime markerTime = new(time.Year, time.Month, 1);
            foreach (Account subAccount in m_SubAccounts.Values)
                subAccount.CreateMarker(markerTime);
            GenerateMarkerFor(markerTime);
        }

        public void AddMarker(DateTime markerTime, double ammount)
        {
            m_Markers[new(markerTime.Year, markerTime.Month, 1)] = ammount;
        }

        public bool NewSubAccount(string name, string description, DateTime? timeLimit = null)
        {
            if (name.Contains('|'))
                return false;
            if (!m_SubAccounts.ContainsKey(name))
            {
                DateTime subAccountTimeLimit;
                if (timeLimit != null && DateTime.Compare((DateTime)timeLimit, m_TimeLimit) >= 0)
                    subAccountTimeLimit = (DateTime)timeLimit;
                else
                    subAccountTimeLimit = m_TimeLimit;
                m_SubAccounts[name] = m_Database.NewAccount(0, string.Format("{0} | {1}", GetName(), name), description, subAccountTimeLimit);
                return true;
            }
            return false;
        }

        public void AddSubAccount(Account account)
        {
            m_SubAccounts[account.GetName().Remove(0, GetName().Length + 3)] = account;
        }

        public Account GetSubAccount(string name)
        {
            return m_SubAccounts[name];
        }

        public Dictionary<DateTime, double> GetMarkers() { return m_Markers; }
        public TransactionStorage GetTransactions() { return m_Transactions; }
        public List<Account> GetSubAccounts() { return m_SubAccounts.Values.ToList(); }
        public DateTime GetTimeLimit() { return m_TimeLimit; }

        public AccountRawData ToRawData()
        {
            AccountRawData rawData = new()
            {
                State = GetState(),
                Amount = GetAmount(),
                Name = GetName(),
                Description = GetDescription(),
                TimeLimit = m_TimeLimit
            };
            foreach (KeyValuePair<DateTime, double> pair in m_Markers)
                rawData.Markers[pair.Key] = pair.Value;
            foreach (Account subAccount in m_SubAccounts.Values)
                rawData.SubAccounts.Add(subAccount.GetName());
            return rawData;
        }
    }
}
