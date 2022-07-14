namespace Ginko
{
    public class AccountRawData: DatabaseObjectRawData
    {
        public readonly Dictionary<DateTime, double> Markers = new();
        public readonly List<string> SubAccounts = new();
        public DateTime TimeLimit { get; set; } = DateTime.MinValue;
    }
}
