namespace Ginko
{
    internal class CodeDrivenReader : IObjectReader
    {
        private readonly Dictionary<string, AccountRawData> m_Accounts = new();
        private readonly List<OperationRawData> m_Operations = new();
        private readonly List<TransactionRawData> m_Transactions = new();

        public void AddAccount(double amount, string name, string description = "", DateTime? timeLimit = null)
        {
            AccountRawData account = new()
            {
                Amount = amount,
                Name = name,
                Description = description,
                TimeLimit = (timeLimit != null) ? (DateTime)timeLimit : DateTime.MinValue
            };
            m_Accounts[account.Name] = account;
        }

        public void AddMarker(string account, DateTime time, double amount)
        {
            if (m_Accounts.ContainsKey(account))
            {
                m_Accounts[account].Markers[time] = amount;
            }
        }

        public void AddSubAccount(string account, double amount, string name, string description = "", DateTime? timeLimit = null)
        {
            if (m_Accounts.ContainsKey(account))
            {
                AccountRawData subAccount = new()
                {
                    Amount = amount,
                    Name = name,
                    Description = description,
                    TimeLimit = (timeLimit != null) ? (DateTime)timeLimit : DateTime.MinValue
                };
                m_Accounts[account].SubAccounts.Add(subAccount.Name);
                m_Accounts[subAccount.Name] = subAccount;
            }
        }

        public void AddOperation(double amount, string name, string description, DateTime nextOperationTime, OperationType operationType, string from = "", string to = "")
        {
            OperationRawData operation = new()
            {
                Amount = amount,
                Name = name,
                Description = description,
                From = from,
                To = to,
                NextOperationTime = nextOperationTime,
                OperationType = operationType
            };
            m_Operations.Add(operation);
        }

        public void AddTransaction(double amount, string name, string description, DateTime time, string from = "", string to = "")
        {
            TransactionRawData transaction = new()
            {
                Amount = amount,
                Name = name,
                Description = description,
                From = from,
                To = to,
                Time = time,
            };
            m_Transactions.Add(transaction);
        }

        public Tuple<List<AccountRawData>, List<OperationRawData>, List<TransactionRawData>> Read()
        {
            return new(m_Accounts.Values.ToList(), m_Operations, m_Transactions);
        }
    }
}
