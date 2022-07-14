namespace Ginko
{
    public class TransactionRawData : DatabaseObjectRawData
    {
        public DateTime Time { get; set; } = DateTime.MinValue;
        public string From { get; set; } = "";
        public string To { get; set; } = "";
    }
}
