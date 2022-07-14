namespace Ginko
{
    public interface IObjectWriter
    {
        public void Write(List<AccountRawData> accounts, List<OperationRawData> operations, List<TransactionRawData> transactions);
    }
}
