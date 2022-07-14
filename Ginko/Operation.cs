namespace Ginko
{
    public enum OperationType
    {
        EVERY_DAY,
        EVERY_MONTH,
        EVERY_YEAR
    }

    public class Operation: DatabaseObject
    {
        private readonly Account? m_From;
        private readonly Account? m_To;
        private DateTime m_NextOperationTime;
        private readonly OperationType m_OperationType;

        public Operation(Database database, double amount, string name, string description, Account? from, Account? to, DateTime nextOperationTime, OperationType operationType): base(database, amount, name, description)
        {
            m_From = from;
            m_To = to;
            m_NextOperationTime = nextOperationTime;
            m_OperationType = operationType;
        }

        public bool ShouldApply()
        {
            if (DateTime.Compare(m_NextOperationTime, DateTime.Today) <= 0)
                return true;
            return false;
        }

        public void ApplyOperation()
        {
            Transaction transaction = m_Database.NewTransaction(GetAmount(), GetName(), GetDescription(), m_NextOperationTime, m_From, m_To);
            if (m_From != null)
                m_From.AddTransaction(transaction);
            if (m_To != null)
                m_To.AddTransaction(transaction);
            switch (m_OperationType)
            {
                case OperationType.EVERY_DAY:
                    {
                        m_NextOperationTime = m_NextOperationTime.AddDays(1);
                        break;
                    }
                case OperationType.EVERY_MONTH:
                    {
                        m_NextOperationTime = m_NextOperationTime.AddMonths(1);
                        break;
                    }
                case OperationType.EVERY_YEAR:
                    {
                        m_NextOperationTime = m_NextOperationTime.AddYears(1);
                        break;
                    }
            }
        }

        public OperationRawData ToRawData()
        {
            OperationRawData rawData = new()
            {
                State = GetState(),
                Amount = GetAmount(),
                Name = GetName(),
                Description = GetDescription(),
                From = (m_From == null) ? "" : m_From.GetName(),
                To = (m_To == null) ? "" : m_To.GetName(),
                NextOperationTime = m_NextOperationTime,
                OperationType = m_OperationType
            };
            return rawData;
        }
    }
}
