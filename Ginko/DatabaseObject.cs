namespace Ginko
{
    public class DatabaseObject
    {
        protected readonly Database m_Database;
        private DatabaseObjectState m_State = DatabaseObjectState.None;

        private double m_Amount;
        private string m_Name;
        private string m_Description;

        public DatabaseObject(Database database, double amount, string name, string description = "")
        {
            m_Database = database;
            m_Amount = amount;
            m_Name = name;
            m_Description = description;
        }

        public void MarkForCreate()
        {
            m_State = DatabaseObjectState.Created;
        }

        public void MarkForDelete()
        {
            m_State = DatabaseObjectState.Deleted;
        }

        public void MarkForUpdate()
        {
            m_State = DatabaseObjectState.Updated;
        }

        public DatabaseObjectState GetState() { return m_State; }
        public double GetAmount() { return m_Amount; }
        public virtual void SetAmount(double amount) { m_Amount = amount; MarkForUpdate(); }
        public string GetName() { return m_Name; }
        public virtual void SetName(string name) { m_Name = name; MarkForUpdate(); }
        public string GetDescription() { return m_Description; }
        public virtual void SetDescription(string description) { m_Description = description; MarkForUpdate(); }
    }
}
