using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HardwareStoreEF
{
    public partial class Shop : Window
    {
        public Shop()
        {
            InitializeComponent();
            Update();
        }

        public void Update()
        {
            using (DBContext db = new DBContext())
            {
                CategoryGrid.ItemsSource = db.Categories.ToList();
                ProductGrid.ItemsSource = (from prod in db.Products
                                           join company in db.Companies on prod.CompanyID equals company.CompanyID
                                           select new
                                           {
                                               Company = company.Name,
                                               prod.Image,
                                               prod.Model,
                                               prod.Price,
                                               prod.Amount
                                           }).ToList();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void ManageCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryManagment category = new CategoryManagment();
            if (category.ShowDialog().Value)
            {
                Update();
            }
        }

        private void ManageProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductManagment product = new ProductManagment();
            if (product.ShowDialog().Value)
            {
                Update();
            }
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            object SelectedProduct = ProductGrid.SelectedItem;
            string company = (ProductGrid.SelectedCells[0].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string model = (ProductGrid.SelectedCells[1].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            using (DBContext db = new DBContext())
            {
                Products pr = db.Products.FirstOrDefault(s => (s.Model == model && s.Companies.Name == company));
                pr.Amount = pr.Amount - 1;
                int ProductID_B = db.Products.FirstOrDefault(o => (o.Model == model && o.Companies.Name == company)).ProductID;
                int UserID_B = db.Users.FirstOrDefault(n => n.Email == NameBlock.Text).UserID;
                if (db.Orders.Any(s => s.ProductID == ProductID_B && s.UserID == UserID_B))
                {
                    Orders order = db.Orders.FirstOrDefault(s => s.ProductID == ProductID_B && s.UserID == UserID_B);
                    order.Amount = order.Amount + 1;
                }
                else
                {
                    db.Orders.Add(new Orders
                    {
                        ProductID = ProductID_B,
                        UserID = UserID_B,
                        Amount = 1
                    });
                }
                db.SaveChanges();
            }
            SelectedProduct = null;
            Update();
        }

        private void CategoryGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object item = CategoryGrid.SelectedItem;
            string category = (CategoryGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
            using (DBContext db = new DBContext())
            {
                ProductGrid.ItemsSource = (from prod in db.Products
                                           join company in db.Companies on prod.CompanyID equals company.CompanyID
                                           join ctg in db.Categories on prod.CategoryID equals ctg.CategoryID
                                           where ctg.Name == category
                                           select new
                                           {
                                               Company = company.Name,
                                               prod.Image,
                                               prod.Model,
                                               prod.Price,
                                               prod.Amount
                                           }).ToList();
            }
        }

        private void ManageCompany_Click(object sender, RoutedEventArgs e)
        {
            CompanyManagment company = new CompanyManagment();
            if (company.ShowDialog().Value)
            {
                Update();
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllBytes("new.chm", Properties.Resources.HelpReference);
            System.Diagnostics.Process.Start(@"new.chm");
        }

        private void ManageOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersManagment orders = new OrdersManagment();
            orders.ShowDialog();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings stt = new Settings();
            stt.EmailBox.Text = NameBlock.Text;
            stt.ShowDialog();
            
        }
    }
}