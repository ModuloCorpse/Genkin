namespace Ginko
{
    public class Database
    {
        private readonly List<Account> m_Accounts = new();
        private readonly List<Operation> m_Operations = new();
        private readonly List<Transaction> m_Transactions = new();

        private IObjectReader? m_Reader = null;
        private IObjectWriter? m_Writer = null;

        private Account AccountFromRawData(AccountRawData rawData)
        {
            Account newAccount = new(this, rawData.Amount, rawData.Name, rawData.Description, rawData.TimeLimit);
            foreach (KeyValuePair<DateTime, double> marker in rawData.Markers)
                newAccount.AddMarker(marker.Key, marker.Value);
            m_Accounts.Add(newAccount);
            return newAccount;
        }

        private void OperationFromRawData(OperationRawData rawData, Dictionary<string, Account> accounts)
        {
            m_Operations.Add(new(this,
                rawData.Amount,
                rawData.Name,
                rawData.Description,
                accounts.TryGetValue(rawData.From, out Account? fromValue) ? fromValue : null,
                accounts.TryGetValue(rawData.To, out Account? toValue) ? toValue : null,
                rawData.NextOperationTime,
                rawData.OperationType));
        }

        private void TransactionFromRawData(TransactionRawData rawData, Dictionary<string, Account> accounts)
        {
            Account? from;
            Account? to;
            accounts.TryGetValue(rawData.From, out from);
            accounts.TryGetValue(rawData.To, out to);
            Transaction transaction = new(this, rawData.Amount, rawData.Name, rawData.Description, rawData.Time, from, to);
            m_Transactions.Add(transaction);
            if (from != null)
                from.AddTransaction(transaction);
            if (to != null)
                to.AddTransaction(transaction);
        }

        public void Read()
        {
            if (m_Reader != null)
            {
                var tuple = m_Reader.Read();
                Dictionary<string, Account> accounts = new Dictionary<string, Account>();
                foreach (AccountRawData accountRawData in tuple.Item1)
                    accounts[accountRawData.Name] = AccountFromRawData(accountRawData);
                foreach (AccountRawData accountRawData in tuple.Item1)
                {
                    Account account = accounts[accountRawData.Name];
                    foreach (string subAccount in accountRawData.SubAccounts)
                    {
                        if (accounts.ContainsKey(subAccount))
                            account.AddSubAccount(accounts[subAccount]);
                    }

                    DateTime now = DateTime.Today;
                    DateTime markerTime = now.AddDays((- now.Day) + 1);
                    account.CreateMarker(markerTime);
                }
                foreach (OperationRawData operationRawData in tuple.Item2)
                    OperationFromRawData(operationRawData, accounts);
                foreach (TransactionRawData transactionRawData in tuple.Item3)
                    TransactionFromRawData(transactionRawData, accounts);
            }
        }

        public void SetReader(IObjectReader reader)
        {
            m_Reader = reader;
        }

        public void Write()
        {
            if (m_Writer != null)
            {
                List<AccountRawData> accountRawDatas = new();
                List<OperationRawData> operationRawDatas = new();
                List<TransactionRawData> transactionRawDatas = new();
                foreach (Account account in m_Accounts)
                    accountRawDatas.Add(account.ToRawData());
                foreach (Operation operation in m_Operations)
                    operationRawDatas.Add(operation.ToRawData());
                foreach (Transaction transaction in m_Transactions)
                    transactionRawDatas.Add(transaction.ToRawData());
                m_Writer.Write(accountRawDatas, operationRawDatas, transactionRawDatas);
            }
        }

        public void SetWriter(IObjectWriter writer)
        {
            m_Writer = writer;
        }

        public Account NewAccount(double ammount, string name, string description = "", DateTime? timeLimit = null)
        {
            Account newAccount = new(this, ammount, name, description, timeLimit);
            m_Accounts.Add(newAccount);
            newAccount.MarkForCreate();
            return newAccount;
        }

        public Operation NewOperation(double amount, string name, string description, Account? from, Account? to, DateTime nextOperationTime, OperationType operationType)
        {
            Operation newOperation = new(this, amount, name, description, from, to, nextOperationTime, operationType);
            m_Operations.Add(newOperation);
            newOperation.MarkForCreate();
            return newOperation;
        }

        public Transaction NewTransaction(double amount, string name, string description, DateTime time, Account? from, Account? to)
        {
            Transaction newTransaction = new(this, amount, name, description, time, from, to);
            m_Transactions.Add(newTransaction);
            newTransaction.MarkForCreate();
            return newTransaction;
        }

        public List<Account> GetAccounts() { return m_Accounts; }
    }
}
