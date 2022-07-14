using System.Runtime.InteropServices;

namespace Ginko
{
    public partial class GinkoForm : Form
    {
        public const int WM_NCHITTEST = 0x84; //Mouse Capture Test
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CLIENT = 0x1; //Application Client Area
        public const int HT_CAPTION = 0x2; //Application Title Bar

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessge(IntPtr hwnd, int wmsg, int wparam, int lparam);

        /*
         * This function intercepts all the commands sent to the application.
         * It checks to see of the message is a mouse click in the application.
         * It passes the action to the base action by default. It reassigns
         * the action to the title bar if it occurred in the client area
         * to allow the drag and move behavior.
        */
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HT_CLIENT)
                        m.Result = (IntPtr)HT_CAPTION;
                    return;
            }

            base.WndProc(ref m);
        }

        private void PanelMouseDown(object? sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessge(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private readonly Database m_Database;

        public GinkoForm(Database database)
        {
            InitializeComponent();
            MenuBar.MouseDown += new MouseEventHandler(PanelMouseDown);
            TabControl.MouseDown += new MouseEventHandler(PanelMouseDown);
            m_Database = database;
            m_Database.Read();

            TabControl.Controls.Clear();
            List<Account> accounts = m_Database.GetAccounts();
            foreach (Account account in accounts)
                AddAccount(account);
        }

        private void AddAccount(Account account)
        {
            TabPage tabPage = new()
            {
                TabIndex = TabControl.Controls.Count,
                Text = account.GetName()
            };
            AccountControl accountControl = new(m_Database, account);
            tabPage.Controls.Add(accountControl);
            TabControl.Controls.Add(tabPage);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}