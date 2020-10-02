using System.Linq;
using System.Windows;

namespace HardwareStoreEF
{
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void GetAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Resources.Code == GetAdminBox.Password)
            {
                using (DBContext db = new DBContext())
                {
                    Users user = db.Users.FirstOrDefault(s => s.Email == EmailBox.Text);
                    if (user.Admin == true)
                    {
                        MessageBox.Show("You already have Admin");
                    }
                    else
                    {
                        user.Admin = true;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                MessageBox.Show("Wrong code");
            }
        }
    }
}