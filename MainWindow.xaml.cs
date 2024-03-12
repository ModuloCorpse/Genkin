using System.Globalization;
using System.IO;
using System.Windows;
using Genkin.Core;

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

        public MainWindow()
        {
            InitializeComponent();
            User? user = User.NewUser("Test", "./test");
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
    }
}