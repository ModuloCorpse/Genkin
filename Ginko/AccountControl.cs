namespace Ginko
{
    public partial class AccountControl : UserControl
    {
        private readonly Database m_Database;
        private readonly Account m_Account;
        private DateTime m_FirstDate;
        private DateTime m_LastDate;
        private readonly DateTimePicker m_DateTimePicker = new();
        private readonly NumericUpDown m_AmountPicker = new();
        private Transaction? m_AmountEditTransaction = null;
        private readonly Dictionary<DataGridViewRow, Transaction> m_RowToTransaction = new();
        private bool m_RowsCleared = false;

        public AccountControl(Database database, Account account)
        {
            InitializeComponent();
            m_Database = database;
            m_Account = account;
            DateTime now = DateTime.Now;
            m_FirstDate = new(now.Year, now.Month, 1);
            m_LastDate = m_FirstDate.AddMonths(1).AddDays(-1);
            BeginDateTimePicker.Value = m_FirstDate;
            EndDateTimePicker.Value = m_LastDate.AddDays(1);
            TransactionList.Controls.Add(m_DateTimePicker);
            m_DateTimePicker.Format = DateTimePickerFormat.Short;
            m_DateTimePicker.CloseUp += new(DateTimePicker_CloseUp);
            m_DateTimePicker.Visible = false;
            TransactionList.Controls.Add(m_AmountPicker);
            m_AmountPicker.DecimalPlaces = 2;
            m_AmountPicker.Validated += new(AmountPicker_Validate);
            m_AmountPicker.Visible = false;
            UpdateCurrentTransactionList();
        }

        private void UpdateCurrentTransactionList()
        {
            if (m_Account != null)
            {
                double amountOnPeriod = 0;
                AmountLabel.Text = string.Format("Montant : {0:0.00} €", m_Account.GetAmmountAt(m_LastDate));
                m_RowToTransaction.Clear();
                m_RowsCleared = true;
                TransactionList.Rows.Clear();
                m_RowsCleared = false;
                List<Transaction> transactions = m_Account.GetTransactions().GetAllTransactionsBetween(m_FirstDate, m_LastDate.AddDays(-1));
                foreach (Transaction transaction in transactions)
                {
                    AddTransactionInView(transaction);
                    if (transaction.GetFrom() == m_Account)
                        amountOnPeriod -= transaction.GetAmount();
                    else
                        amountOnPeriod += transaction.GetAmount();
                }
                AmountOnPeriodLabel.Text = string.Format("Montant sur la période : {0:0.00} €", amountOnPeriod);
            }
        }

        private void ValidateAmount()
        {
            m_AmountPicker.Visible = false;
            if (m_AmountEditTransaction != null)
            {
                double newAmount = (double)m_AmountPicker.Value;
                if (newAmount != m_AmountEditTransaction.GetAmount())
                {
                    m_AmountEditTransaction.SetAmount(newAmount);
                    UpdateCurrentTransactionList();
                }
            }
        }

        private void TransactionList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_AmountPicker.Visible)
                ValidateAmount();

            m_AmountEditTransaction = null;
            m_AmountPicker.Visible = false;
            m_DateTimePicker.Visible = false;

            Rectangle rectangle = TransactionList.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            if (e.ColumnIndex == 2)
            {
                m_DateTimePicker.Size = new(rectangle.Width, rectangle.Height);
                m_DateTimePicker.Location = new(rectangle.X, rectangle.Y);
                m_DateTimePicker.Visible = true;
            }
            else if (e.ColumnIndex == 1)
            {
                if (m_RowToTransaction.TryGetValue(TransactionList.Rows[e.RowIndex], out Transaction? transaction))
                {
                    m_AmountEditTransaction = transaction;
                    m_AmountPicker.Value = (decimal)transaction.GetAmount();
                    m_AmountPicker.Size = new(rectangle.Width, rectangle.Height);
                    m_AmountPicker.Location = new(rectangle.X, rectangle.Y);
                    m_AmountPicker.Visible = true;
                }
            }
        }

        private void TransactionList_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (m_RowsCleared)
                return;
            bool needUpdate = false;
            foreach (KeyValuePair<DataGridViewRow, Transaction> keyPair in m_RowToTransaction)
            {
                if (TransactionList.Rows.IndexOf(keyPair.Key) == -1)
                {
                    keyPair.Value.Delete();
                    needUpdate = true;
                }
            }
            if (needUpdate)
                UpdateCurrentTransactionList();
        }

        private void DateTimePicker_CloseUp(object? sender, EventArgs e)
        {
            m_DateTimePicker.Visible = false;
            if (m_RowToTransaction.TryGetValue(TransactionList.CurrentRow, out Transaction? transaction))
            {
                DateTime newTime = m_DateTimePicker.Value;
                if (transaction.GetTime() != newTime)
                {
                    transaction.SetTime(newTime);
                    UpdateCurrentTransactionList();
                }
            }
        }

        private void AmountPicker_Validate(object? sender, EventArgs e)
        {
            ValidateAmount();
        }

        private void AddTransactionInView(Transaction transaction)
        {
            string searchBoxText = SearchBox.Text;
            if (searchBoxText != "" &&
                !transaction.GetName().Contains(searchBoxText) &&
                !transaction.GetDescription().Contains(searchBoxText))
                return;
            DataGridViewRow transactionRow = new();
            transactionRow.Cells.Add(new DataGridViewTextBoxCell());
            transactionRow.Cells.Add(new DataGridViewTextBoxCell());
            transactionRow.Cells.Add(new DataGridViewTextBoxCell());
            transactionRow.Cells.Add(new DataGridViewTextBoxCell());
            if (transaction.GetFrom() == m_Account)
            {
                transaction.SetFromRow(transactionRow);
                transactionRow.DefaultCellStyle.BackColor = Color.PaleVioletRed;
            }
            else
            {
                transaction.SetToRow(transactionRow);
                transactionRow.DefaultCellStyle.BackColor = Color.PaleGreen;
            }
            m_RowToTransaction[transactionRow] = transaction;
            TransactionList.Rows.Add(transactionRow);
        }

        private void BeginDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            m_FirstDate = BeginDateTimePicker.Value;
            UpdateCurrentTransactionList();
        }

        private void EndDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            m_LastDate = EndDateTimePicker.Value.AddDays(-1);
            UpdateCurrentTransactionList();
        }

        private void PreviousMonthButton_Click(object sender, EventArgs e)
        {
            DateTime beginTime = BeginDateTimePicker.Value;
            DateTime endTime = EndDateTimePicker.Value;
            DateTime previousBeginTime = beginTime.AddMonths(-1);
            DateTime previousEndTime = endTime.AddMonths(-1);
            if (DateTime.Compare(previousBeginTime, m_Account.GetTimeLimit()) >= 0)
            {
                m_FirstDate = previousBeginTime;
                m_LastDate = previousEndTime.AddDays(-1);
                BeginDateTimePicker.Value = previousBeginTime;
                EndDateTimePicker.Value = previousEndTime;
                UpdateCurrentTransactionList();
            }
        }

        private void NextMonthButton_Click(object sender, EventArgs e)
        {
            DateTime beginTime = BeginDateTimePicker.Value;
            DateTime endTime = EndDateTimePicker.Value;
            DateTime previousBeginTime = beginTime.AddMonths(1);
            DateTime previousEndTime = endTime.AddMonths(1);
            m_FirstDate = previousBeginTime;
            m_LastDate = previousEndTime.AddDays(-1);
            BeginDateTimePicker.Value = previousBeginTime;
            EndDateTimePicker.Value = previousEndTime;
            UpdateCurrentTransactionList();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            UpdateCurrentTransactionList();
        }
    }
}
