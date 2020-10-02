using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HardwareStoreEF
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            Close();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            textBoxFirstName.Clear();
            textBoxLastName.Clear();
            textBoxEmail.Clear();
            textBoxAddress.Clear();
            passwordBox.Clear();
            passwordBoxConfirm.Clear();
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxEmail.Text.Length == 0)
            {
                errormessage.Foreground = Brushes.DarkRed;
                errormessage.Text = "Enter an email";
                textBoxEmail.Focus();
            }
            else if (!Regex.IsMatch(textBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                errormessage.Text = "Enter a valid email";
                textBoxEmail.Select(0, textBoxEmail.Text.Length);
                textBoxEmail.Focus();
            }
            else
            {
                string firstName = textBoxFirstName.Text;
                string lastName = textBoxLastName.Text;
                string email = textBoxEmail.Text;
                string password = passwordBox.Password;
                string address = textBoxAddress.Text;

                if (passwordBox.Password.Length == 0)
                {
                    errormessage.Text = "Enter password";
                    passwordBox.Focus();
                }
                else if (passwordBoxConfirm.Password.Length == 0)
                {
                    errormessage.Text = "Enter Confirm password";
                    passwordBoxConfirm.Focus();
                }
                else if (passwordBox.Password != passwordBoxConfirm.Password)
                {
                    errormessage.Text = "Confirm password must be same as password";
                    passwordBoxConfirm.Focus();
                }
                else
                {
                    errormessage.Text = "";
                    using (DBContext db = new DBContext())
                    {
                        if (db.Users.Any(s => s.Email == email))
                        {
                            MessageBox.Show("That account arleady exist");
                            return;
                        }
                        db.Users.Add(new Users { FirstName = firstName, LastName = lastName, Email = email, Password = password, Address = address });
                        db.SaveChanges();
                    }
                    errormessage.Text = "Successfully";
                    Reset();
                }
            }
        }
    }
}