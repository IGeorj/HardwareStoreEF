using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HardwareStoreEF
{
    public partial class CategoryManagment : Window
    {
        public CategoryManagment()
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
                CategoryGrid.ItemsSource = db.Categories.ToList();
                CategoryComboBoxChange.ItemsSource = db.Categories.ToList();
                CategoryComboBoxDelete.ItemsSource = db.Categories.ToList();
                CategoryComboBoxChange.DisplayMemberPath = "Name";
                CategoryComboBoxDelete.DisplayMemberPath = "Name";
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
                if (db.Categories.Any(s => s.Name == AddBlock.Text))
                {
                    MessageBox.Show("This category already exist");
                    return;
                }
                db.Categories.Add(new Categories { Name = AddBlock.Text });
                db.SaveChanges();
            }
            Update();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryComboBoxChange.Text == "" || ChangeBlock.Text == "")
            {
                MessageBox.Show("Please input all data");
                return;
            }
            using (DBContext db = new DBContext())
            {
                if (db.Categories.Any(s => s.Name == ChangeBlock.Text))
                {
                    MessageBox.Show("This category already exist");
                    return;
                }
                Categories C1 = db.Categories.FirstOrDefault(s => s.Name == CategoryComboBoxChange.Text);
                C1.Name = ChangeBlock.Text;
                db.SaveChanges();
            }
            CategoryComboBoxChange.Text = "";
            ChangeBlock.Text = "";
            Update();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryComboBoxDelete.Text == "")
            {
                MessageBox.Show("Please select a category");
                return;
            }
            using (DBContext db = new DBContext())
            {
                db.Categories.Remove(db.Categories.FirstOrDefault(s => s.Name == CategoryComboBoxDelete.Text));
                db.SaveChanges();
            }
            CategoryComboBoxDelete.Text = "";
            Update();
        }
    }
}