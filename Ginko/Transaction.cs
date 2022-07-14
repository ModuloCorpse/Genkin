namespace Ginko
{
    public class Transaction: DatabaseObject
    {
        private static readonly string K_FROM_ROW_FORMAT = "-{0} €";
        private static readonly string K_TO_ROW_FORMAT = "{0} €";

        private DataGridViewRow? m_FromRow;
        private DataGridViewRow? m_ToRow;
        private DateTime m_Time;
        private readonly Account? m_From;
        private readonly Account? m_To;

        public Transaction(Database database, double amount, string name, string description, DateTime time, Account? from, Account? to): base(database, amount, name, description)
        {
            m_Time = time;
            m_From = from;
            m_To = to;
        }

        private void UpdateFromRow()
        {
            if (m_FromRow != null)
            {
                m_FromRow.Cells[0].Value = GetName();
                m_FromRow.Cells[1].Value = string.Format(K_FROM_ROW_FORMAT, GetAmount());
                m_FromRow.Cells[2].Value = m_Time.ToString("dd/MM/yyyy");
                m_FromRow.Cells[3].Value = GetDescription();
            }
        }

        public void SetFromRow(DataGridViewRow fromRow)
        {
            m_FromRow = fromRow;
            UpdateFromRow();
        }

        private void UpdateToRow()
        {
            if (m_ToRow != null)
            {
                m_ToRow.Cells[0].Value = GetName();
                m_ToRow.Cells[1].Value = string.Format(K_TO_ROW_FORMAT, GetAmount());
                m_ToRow.Cells[2].Value = m_Time.ToString("dd/MM/yyyy");
                m_ToRow.Cells[3].Value = GetDescription();
            }
        }

        public void SetToRow(DataGridViewRow toRow)
        {
            m_ToRow = toRow;
            UpdateToRow();
        }

        public DateTime GetTime() { return m_Time; }
        public Account? GetFrom() { return m_From; }
        public Account? GetTo() { return m_To; }

        private void UpdateAccountMarkers(DateTime time)
        {
            DateTime markerTime = new(time.Year, time.Month, 1);
            if (time.Day != 1)
                markerTime = markerTime.AddMonths(1);
            if (m_From != null)
                m_From.UpdateMarker(markerTime);
            if (m_To != null)
                m_To.UpdateMarker(markerTime);
        }

        private void RemoveTransaction()
        {
            if (m_From != null)
                m_From.RemoveTransaction(this);
            if (m_To != null)
                m_To.RemoveTransaction(this);
        }

        private void AddTransaction()
        {
            if (m_From != null)
                m_From.AddTransaction(this);
            if (m_To != null)
                m_To.AddTransaction(this);
        }

        public void Delete()
        {
            RemoveTransaction();
            MarkForDelete();
            UpdateAccountMarkers(m_Time);
        }

        public void Restore()
        {
            AddTransaction();
            MarkForUpdate();
            UpdateAccountMarkers(m_Time);
        }

        public override void SetAmount(double amount)
        {
            if (GetAmount() == amount)
                return;
            base.SetAmount(amount);
            UpdateFromRow();
            UpdateToRow();
            UpdateAccountMarkers(m_Time);
        }

        public override void SetName(string name)
        {
            base.SetName(name);
            UpdateFromRow();
            UpdateToRow();
        }

        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            UpdateFromRow();
            UpdateToRow();
        }

        public void SetTime(DateTime time)
        {
            if (time == m_Time)
                return;
            RemoveTransaction();
            DateTime markerTimeToUpdate = (DateTime.Compare(time, m_Time) > 0) ? m_Time : time;
            m_Time = time;
            MarkForUpdate();
            UpdateFromRow();
            UpdateToRow();
            AddTransaction();
            UpdateAccountMarkers(markerTimeToUpdate);
        }

        private void FromRow_SetName(string name)
        {
            base.SetName(name);
            UpdateToRow();
        }

        private void FromRow_SetDescription(string description)
        {
            base.SetDescription(description);
            UpdateToRow();
        }

        private void ToRow_SetName(string name)
        {
            base.SetName(name);
            UpdateFromRow();
        }

        private void ToRow_SetDescription(string description)
        {
            base.SetDescription(description);
            UpdateFromRow();
        }

        public TransactionRawData ToRawData()
        {
            TransactionRawData rawData = new()
            {
                State = GetState(),
                Amount = GetAmount(),
                Name = GetName(),
                Description = GetDescription(),
                Time = m_Time,
                From = (m_From == null) ? "" : m_From.GetName(),
                To = (m_To == null) ? "" : m_To.GetName()
            };
            return rawData;
        }
    }
}
