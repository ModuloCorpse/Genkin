namespace Ginko
{
    public class CodeDrivenWriter : IObjectWriter
    {
        private static void ShowDatabaseObjectUpdates(DatabaseObjectRawData obj)
        {
            switch (obj.State)
            {
                case DatabaseObjectState.Created:
                    {
                        System.Diagnostics.Debug.WriteLine("{0} Created", obj.Name);
                        break;
                    }
                case DatabaseObjectState.Updated:
                    {
                        System.Diagnostics.Debug.WriteLine("{0} Updated", obj.Name);
                        break;
                    }
                case DatabaseObjectState.Deleted:
                    {
                        System.Diagnostics.Debug.WriteLine("{0} Deleted", obj.Name);
                        break;
                    }
                default:
                    {
                        System.Diagnostics.Debug.WriteLine("{0} Unchanged", obj.Name);
                        break;
                    }
            }
        }

        public void Write(List<AccountRawData> accounts, List<OperationRawData> operations, List<TransactionRawData> transactions)
        {
            foreach (AccountRawData account in accounts)
                ShowDatabaseObjectUpdates(account);
            foreach (OperationRawData operation in operations)
                ShowDatabaseObjectUpdates(operation);
            foreach (TransactionRawData transaction in transactions)
                ShowDatabaseObjectUpdates(transaction);
        }
    }
}
