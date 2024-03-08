using System.Windows;

namespace Ginko
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Account account = new("Test", "Test account", 100, Guid.NewGuid());
            Saving saving = account[0];
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
                        decimal amount = Convert.ToDecimal((rand.NextDouble() * 99.0) + 1.0);
                        Transaction transaction = new(string.Format("Test {0}", i), string.Empty, date, amount, Guid.NewGuid());
                        saving.AddTransaction(transaction);
                        i++;
                    }
                } while (percent <= 30);
                date = date.AddDays(1);
            }

            Console.WriteLine(account);
        }
    }
}