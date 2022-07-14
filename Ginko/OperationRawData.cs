namespace Ginko
{
    public class OperationRawData : DatabaseObjectRawData
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public DateTime NextOperationTime { get; set; } = DateTime.MinValue;
        public OperationType OperationType { get; set; } = OperationType.EVERY_YEAR;
    }
}
