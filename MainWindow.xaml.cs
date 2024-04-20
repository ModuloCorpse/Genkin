using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CorpseLib.DataNotation;
using CorpseLib.Json;
using Genkin.Core;
using DataObject = CorpseLib.DataNotation.DataObject;

namespace Genkin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string[] ms_CurrencySymbols = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => culture.NumberFormat.CurrencySymbol).Distinct().ToArray();

        private static void FillSaving(Saving saving)
        {
            DateTime date = new(2000, 1, 1);
            DateTime endDate = new(2002, 1, 1);

            int i = 0;
            Random rand = new();
            while (date < endDate)
            {
                int percent;
                do
                {
                    percent = rand.Next(101);
                    if (percent <= 30)
                    {
                        saving.NewTransaction(string.Format("Test {0}", i), string.Empty, date, Math.Round(Convert.ToDecimal((rand.NextDouble() * 99.0) + 1.0), 2, MidpointRounding.AwayFromZero));
                        i++;
                    }
                } while (percent <= 30);
                date = date.AddDays(1);
            }
        }

        static MainWindow()
        {
            DataHelper.RegisterSerializer(new UserInfo.DataSerializer());
        }

        private readonly List<UserInfo> m_Users = [];

        public MainWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;

            if (File.Exists("settings.json"))
            {
                DataObject settings = JsonParser.LoadFromFile("settings.json");
                m_Users = settings.GetList<UserInfo>("users");
            }

            foreach (UserInfo info in m_Users)
                UserScrollViewer.Children.Add(new UserScrollItem(info));

            //Testing purpous
            User? user = User.NewUser("./test");
            if (user != null)
            {
                user.SetSavePassword("Password");
                Account account = user.NewAccount("Test", "Test account", "$");
                FillSaving(account.NewSaving("Test 1", "Test saving 1", 100));
                FillSaving(account.NewSaving("Test 2", "Test saving 2", 200));

                Console.WriteLine(user);
                user.Save();

                List<User> users = User.LoadUsers("./test");
                foreach (User loadedUser in users)
                {
                    loadedUser.SetLoadPassword("Password");
                    loadedUser.Load();
                    Console.WriteLine("=========================Loaded user=========================");
                    Console.WriteLine(loadedUser);
                }

                Directory.Delete("./test", true);
            }
        }

        private void AddUserInfo(UserInfo info)
        {
            m_Users.Add(info);
            UserScrollViewer.Children.Add(new UserScrollItem(info));
        }

        private void NewUserInfo(string name, string path, Rect rect) => AddUserInfo(new(name, path, rect, Guid.NewGuid()));
        private void NewUserInfo(string name, string defaultAvatar) => AddUserInfo(new(name, defaultAvatar, new(0, 0, 85, 85), Guid.NewGuid()));

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                if (WindowState == WindowState.Maximized)
                    SystemCommands.RestoreWindow(this);
                else
                    SystemCommands.MaximizeWindow(this);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;
            if (Properties.Settings.Default.Maximized)
                WindowState = WindowState.Maximized;
            Activate();
            Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = Top;
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Height = Height;
                Properties.Settings.Default.Width = Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();

            DataObject settings = new()
            {
                { "users", m_Users }
            };

            JsonParser.WriteToFile("settings.json", settings);
        }
    }
}