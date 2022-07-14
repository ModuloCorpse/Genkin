namespace Ginko
{
    partial class AccountControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TransactionList = new System.Windows.Forms.DataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AmountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BeginDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.EndDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.AmountOnPeriodLabel = new System.Windows.Forms.Label();
            this.AmountLabel = new System.Windows.Forms.Label();
            this.PreviousMonthButton = new System.Windows.Forms.Button();
            this.NextMonthButton = new System.Windows.Forms.Button();
            this.SearchBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.TransactionList)).BeginInit();
            this.SuspendLayout();
            // 
            // TransactionList
            // 
            this.TransactionList.AllowUserToAddRows = false;
            this.TransactionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TransactionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.AmountColumn,
            this.TimeColumn,
            this.DescriptionColumn});
            this.TransactionList.Location = new System.Drawing.Point(3, 56);
            this.TransactionList.Name = "TransactionList";
            this.TransactionList.RowTemplate.Height = 25;
            this.TransactionList.Size = new System.Drawing.Size(784, 331);
            this.TransactionList.TabIndex = 0;
            this.TransactionList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TransactionList_CellContentClick);
            this.TransactionList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TransactionList_CellContentClick);
            this.TransactionList.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.TransactionList_RowsRemoved);
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Nom";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Width = 150;
            // 
            // AmountColumn
            // 
            this.AmountColumn.HeaderText = "Montant";
            this.AmountColumn.Name = "AmountColumn";
            // 
            // TimeColumn
            // 
            this.TimeColumn.HeaderText = "Date";
            this.TimeColumn.Name = "TimeColumn";
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.HeaderText = "Description";
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.Width = 370;
            // 
            // BeginDateTimePicker
            // 
            this.BeginDateTimePicker.Location = new System.Drawing.Point(46, 27);
            this.BeginDateTimePicker.Name = "BeginDateTimePicker";
            this.BeginDateTimePicker.Size = new System.Drawing.Size(200, 23);
            this.BeginDateTimePicker.TabIndex = 1;
            this.BeginDateTimePicker.CloseUp += new System.EventHandler(this.BeginDateTimePicker_CloseUp);
            // 
            // EndDateTimePicker
            // 
            this.EndDateTimePicker.Location = new System.Drawing.Point(337, 27);
            this.EndDateTimePicker.Name = "EndDateTimePicker";
            this.EndDateTimePicker.Size = new System.Drawing.Size(200, 23);
            this.EndDateTimePicker.TabIndex = 2;
            this.EndDateTimePicker.CloseUp += new System.EventHandler(this.EndDateTimePicker_CloseUp);
            // 
            // AmountOnPeriodLabel
            // 
            this.AmountOnPeriodLabel.AutoSize = true;
            this.AmountOnPeriodLabel.Location = new System.Drawing.Point(337, 9);
            this.AmountOnPeriodLabel.Name = "AmountOnPeriodLabel";
            this.AmountOnPeriodLabel.Size = new System.Drawing.Size(187, 15);
            this.AmountOnPeriodLabel.TabIndex = 3;
            this.AmountOnPeriodLabel.Text = "Montant sur la période : XXXXXX €";
            // 
            // AmountLabel
            // 
            this.AmountLabel.AutoSize = true;
            this.AmountLabel.Location = new System.Drawing.Point(46, 9);
            this.AmountLabel.Name = "AmountLabel";
            this.AmountLabel.Size = new System.Drawing.Size(113, 15);
            this.AmountLabel.TabIndex = 4;
            this.AmountLabel.Text = "Montant : XXXXXX €";
            // 
            // PreviousMonthButton
            // 
            this.PreviousMonthButton.Location = new System.Drawing.Point(252, 27);
            this.PreviousMonthButton.Name = "PreviousMonthButton";
            this.PreviousMonthButton.Size = new System.Drawing.Size(23, 23);
            this.PreviousMonthButton.TabIndex = 5;
            this.PreviousMonthButton.Text = "<";
            this.PreviousMonthButton.UseVisualStyleBackColor = true;
            this.PreviousMonthButton.Click += new System.EventHandler(this.PreviousMonthButton_Click);
            // 
            // NextMonthButton
            // 
            this.NextMonthButton.Location = new System.Drawing.Point(308, 27);
            this.NextMonthButton.Name = "NextMonthButton";
            this.NextMonthButton.Size = new System.Drawing.Size(23, 23);
            this.NextMonthButton.TabIndex = 6;
            this.NextMonthButton.Text = ">";
            this.NextMonthButton.UseVisualStyleBackColor = true;
            this.NextMonthButton.Click += new System.EventHandler(this.NextMonthButton_Click);
            // 
            // SearchBox
            // 
            this.SearchBox.Location = new System.Drawing.Point(632, 9);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(155, 23);
            this.SearchBox.TabIndex = 7;
            this.SearchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // AccountControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SearchBox);
            this.Controls.Add(this.NextMonthButton);
            this.Controls.Add(this.PreviousMonthButton);
            this.Controls.Add(this.AmountLabel);
            this.Controls.Add(this.AmountOnPeriodLabel);
            this.Controls.Add(this.EndDateTimePicker);
            this.Controls.Add(this.BeginDateTimePicker);
            this.Controls.Add(this.TransactionList);
            this.Name = "AccountControl";
            this.Size = new System.Drawing.Size(790, 390);
            ((System.ComponentModel.ISupportInitialize)(this.TransactionList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView TransactionList;
        private DateTimePicker BeginDateTimePicker;
        private DateTimePicker EndDateTimePicker;
        private DataGridViewTextBoxColumn NameColumn;
        private DataGridViewTextBoxColumn AmountColumn;
        private DataGridViewTextBoxColumn TimeColumn;
        private DataGridViewTextBoxColumn DescriptionColumn;
        private Label AmountOnPeriodLabel;
        private Label AmountLabel;
        private Button PreviousMonthButton;
        private Button NextMonthButton;
        private TextBox SearchBox;
    }
}
