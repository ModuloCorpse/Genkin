namespace Ginko
{
    public interface IObjectReader
    {
        public Tuple<List<AccountRawData>, List<OperationRawData>, List<TransactionRawData>> Read();
    }
}
