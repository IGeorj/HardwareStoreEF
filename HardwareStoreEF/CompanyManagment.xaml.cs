using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HardwareStoreEF
{
    public partial class CompanyManagment : Window
    {
        public CompanyManagment()
        {
            InitializeComponent();
            Update();
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            this.DialogResult = true;
        }

        public void Update()
        {
            using (DBContext db = new DBContext())
            {
                CompanyGrid.ItemsSource = db.Companies.ToList();
                CompanyComboBoxChange.ItemsSource = db.Companies.ToList();
                CompanyComboBoxDelete.ItemsSource = db.Companies.ToList();
                CompanyComboBoxChange.DisplayMemberPath = "Name";
                CompanyComboBoxDelete.DisplayMemberPath = "Name";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness((150 * index), 45, 0, 360);

            switch (index)
            {
                case 0:
                    AddPanel.Visibility = Visibility.Visible;
                    ChangePanel.Visibility = Visibility.Hidden;
                    DeletePanel.Visibility = Visibility.Hidden;
                    break;

                case 1:
                    AddPanel.Visibility = Visibility.Hidden;
                    ChangePanel.Visibility = Visibility.Visible;
                    DeletePanel.Visibility = Visibility.Hidden;
                    break;

                case 2:
                    AddPanel.Visibility = Visibility.Hidden;
                    ChangePanel.Visibility = Visibility.Hidden;
                    DeletePanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (AddBlock.Text == "")
            {
                MessageBox.Show("Insert a Name");
                return;
            }
            using (DBContext db = new DBContext())
            {
                if (db.Companies.Any(s => s.Name == AddBlock.Text))
                {
                    MessageBox.Show("This company already exist");
                    return;
                }
                db.Companies.Add(new Companies { Name = AddBlock.Text });
                db.SaveChanges();
            }
            Update();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (CompanyComboBoxChange.Text == "" || ChangeBlock.Text == "")
            {
                MessageBox.Show("Please input all data");
                return;
            }
            using (DBContext db = new DBContext())
            {
                if (db.Companies.Any(s => s.Name == ChangeBlock.Text))
                {
                    MessageBox.Show("This company already exist");
                    return;
                }
                Companies C1 = db.Companies.FirstOrDefault(s => s.Name == CompanyComboBoxChange.Text);
                C1.Name = ChangeBlock.Text;
                db.SaveChanges();
            }
            CompanyComboBoxChange.Text = "";
            ChangeBlock.Text = "";
            Update();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CompanyComboBoxDelete.Text == "")
            {
                MessageBox.Show("Please input a company");
                return;
            }
            using (DBContext db = new DBContext())
            {
                db.Companies.Remove(db.Companies.FirstOrDefault(s => s.Name == CompanyComboBoxDelete.Text));
                db.SaveChanges();
            }
            CompanyComboBoxDelete.Text = "";
            Update();
        }
    }
}