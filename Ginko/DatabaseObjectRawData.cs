namespace Ginko
{
    public class DatabaseObjectRawData
    {
        public DatabaseObjectState State { get; set; } = DatabaseObjectState.None;
        public double Amount { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
