using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HardwareStoreEF
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            LogBox.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            Registration reg = new Registration();
            reg.Show();
            Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            using (DBContext db = new DBContext())
            {
                if (LogBox.Text.Length == 0)
                {
                    MessageBox.Show("Enter an email.");
                    LogBox.Focus();
                }
                else if (!Regex.IsMatch(LogBox.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    MessageBox.Show("Enter a valid email.");
                    LogBox.Select(0, LogBox.Text.Length);
                    LogBox.Focus();
                }
                else
                {
                    string email = LogBox.Text;
                    string password = PassBox.Password;

                    if (db.Users.Any(s => s.Email == email && s.Password == password))
                    {
                        Shop sp = new Shop();
                        sp.NameBlock.Text = db.Users.FirstOrDefault(s => s.Email == email && s.Password == password).Email;
                        if (db.Users.FirstOrDefault(s => s.Email == email && s.Password == password).Admin == true)
                        {
                            sp.Administration.Visibility = Visibility.Visible;
                        }
                        sp.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Sorry! Please enter existing email/password.");
                    }
                }
            }
            LoginButton.IsEnabled = true;
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);
            }
        }
    }
}