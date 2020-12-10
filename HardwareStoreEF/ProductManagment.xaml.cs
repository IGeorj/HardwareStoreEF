using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace HardwareStoreEF
{
    public partial class ProductManagment : Window
    {
        private object SelectedProduct = null;
        private int CursorSize = 130;
        private int CursorIndex;
        private bool MenuClosed = false;
        private BitmapImage image;
        private BitmapImage changeImage;
        public BitmapImage Image
        {
            get { return image; }
            set
            {
                image = value;
            }
        }
        public BitmapImage ChangeImage
        {
            get { return changeImage; }
            set
            {
                changeImage = value;
            }
        }
        public ProductManagment()
        {
            InitializeComponent();
            using (DBContext db = new DBContext())
            {
                CategoryGrid.ItemsSource = db.Categories.ToList();
            }
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
                ProductGrid.ItemsSource = (from prod in db.Products
                                           join company in db.Companies on prod.CompanyID equals company.CompanyID
                                           select new
                                           {
                                               Company = company.Name,
                                               prod.Model,
                                               prod.Price,
                                               prod.Amount
                                           }).ToList();
                AddCategoryBox.ItemsSource = db.Categories.ToList();
                AddCategoryBox.DisplayMemberPath = "Name";
                AddCompanyBox.ItemsSource = db.Companies.ToList();
                AddCompanyBox.DisplayMemberPath = "Name";
                ChangeCategoryBox.ItemsSource = db.Categories.ToList();
                ChangeCategoryBox.DisplayMemberPath = "Name";
                ChangeCompanyBox.ItemsSource = db.Companies.ToList();
                ChangeCompanyBox.DisplayMemberPath = "Name";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CursorIndex = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness((CursorSize * CursorIndex), 45, 0, 360);

            switch (CursorIndex)
            {
                case 0:
                    AddPanel1.Visibility = Visibility.Visible;
                    AddPanel2.Visibility = Visibility.Visible;
                    ChangePanel1.Visibility = Visibility.Hidden;
                    ChangePanel2.Visibility = Visibility.Hidden;
                    DeletePanel.Visibility = Visibility.Hidden;
                    break;

                case 1:
                    AddPanel1.Visibility = Visibility.Hidden;
                    AddPanel2.Visibility = Visibility.Hidden;
                    ChangePanel1.Visibility = Visibility.Visible;
                    ChangePanel2.Visibility = Visibility.Visible;
                    DeletePanel.Visibility = Visibility.Hidden;
                    break;

                case 2:
                    AddPanel1.Visibility = Visibility.Hidden;
                    AddPanel2.Visibility = Visibility.Hidden;
                    ChangePanel1.Visibility = Visibility.Hidden;
                    ChangePanel2.Visibility = Visibility.Hidden;
                    DeletePanel.Visibility = Visibility.Visible;
                    break;
            }
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
                                               prod.Model,
                                               prod.Price,
                                               prod.Amount
                                           }).ToList();
            }
        }

        private void SideBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuClosed)
            {
                Storyboard openMenu = (Storyboard)button.FindResource("OpenMenu");
                openMenu.Begin();
                CategoryGrid.Visibility = Visibility.Visible;
                CursorSize = 132;
                GridCursor.Width = 132;
                GridCursor.Margin = new Thickness((CursorSize * CursorIndex), 45, 0, 360);
                Butt1.Width = 132;
                Butt2.Width = 132;
                Butt3.Width = 132;
            }
            else
            {
                Storyboard closeMenu = (Storyboard)button.FindResource("CloseMenu");
                closeMenu.Begin();
                CategoryGrid.Visibility = Visibility.Hidden;
                CursorSize = 152;
                GridCursor.Width = 152;
                GridCursor.Margin = new Thickness((CursorSize * CursorIndex), 45, 0, 360);
                Butt1.Width = 152;
                Butt2.Width = 152;
                Butt3.Width = 152;
            }

            MenuClosed = !MenuClosed;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddModelBlock.Text == "" || AddAmountBlock.Text == "" || AddPriceBlock.Text == "" || AddCategoryBox.Text == "" || AddCompanyBox.Text == "")
            {
                MessageBox.Show("Insert all data");
                return;
            }
            using (DBContext db = new DBContext())
            {
                if (db.Products.Any(s => (s.Model == AddModelBlock.Text && s.Companies.Name == AddCompanyBox.Text)))
                {
                    MessageBox.Show("This product already exist");
                    return;
                }
                db.Products.Add(new Products
                {
                    Image = $"{Image.UriSource}",
                    Model = AddModelBlock.Text,
                    Amount = int.Parse(AddAmountBlock.Text),
                    Price = int.Parse(AddPriceBlock.Text),
                    CategoryID = db.Categories.FirstOrDefault(s => s.Name == AddCategoryBox.Text).CategoryID,
                    CompanyID = db.Companies.FirstOrDefault(s => s.Name == AddCompanyBox.Text).CompanyID
                });
                db.SaveChanges();
            }
            Update();
            image = null;
            borderImage.Source = null;
            borderImage.Source = image;
        }

        public void ClearBox()
        {
            ChangeAmountBlock.Text = "";
            ChangePriceBlock.Text = "";
            ChangeModelBlock.Text = "";
            ChangeCompanyBox.Text = "";
            ChangeCategoryBox.Text = "";
            DeleteCompanylBlock.Text = "";
            DeleteModelBlock.Text = "";
            SelectedProduct = null;
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Please select a product");
                return;
            }
            string company = (ProductGrid.SelectedCells[0].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string model = (ProductGrid.SelectedCells[1].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            using (DBContext db = new DBContext())
            {
                if (db.Products.Any(s => (s.Model == AddModelBlock.Text && s.Companies.Name == AddCompanyBox.Text)))
                {
                    MessageBox.Show("This product already exist");
                    return;
                }
                Products pr = db.Products.FirstOrDefault(s => (s.Model == model && s.Companies.Name == company));
                pr.Image = $"{changeImage.UriSource}";
                pr.Amount = int.Parse(ChangeAmountBlock.Text);
                pr.Price = int.Parse(ChangePriceBlock.Text);
                pr.Model = ChangeModelBlock.Text;
                pr.Companies = db.Companies.FirstOrDefault(s => s.Name == ChangeCompanyBox.Text);
                pr.Categories = db.Categories.FirstOrDefault(s => s.Name == ChangeCategoryBox.Text);
                db.SaveChanges();
            }
            ClearBox();
            SelectedProduct = null;
            Update();
            changeImage = null;
            borderImage2.Source = null;
            borderImage2.Source = changeImage;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Please select a product");
                return;
            }
            string company = (ProductGrid.SelectedCells[0].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string model = (ProductGrid.SelectedCells[1].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            using (DBContext db = new DBContext())
            {
                db.Products.Remove(db.Products.FirstOrDefault(s => (s.Model == model && s.Companies.Name == company)));
                db.SaveChanges();
            }
            ClearBox();
            Update();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedProduct = ProductGrid.SelectedItem;
            string company = (ProductGrid.SelectedCells[0].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string model = (ProductGrid.SelectedCells[1].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            if (ChangePanel1.Visibility == Visibility.Hidden && AddPanel1.Visibility == Visibility.Hidden)
            {
                DeleteCompanylBlock.Text = company;
                DeleteModelBlock.Text = model;
                return;
            }
            using (DBContext db = new DBContext())
            {
                Products pr = db.Products.FirstOrDefault(s => (s.Model == model && s.Companies.Name == company));
                borderImage2.Source = null;
                changeImage = new BitmapImage(new Uri(pr.Image, UriKind.Absolute));
                borderImage2.Source = changeImage;
                ChangeAmountBlock.Text = pr.Amount.ToString();
                ChangePriceBlock.Text = pr.Price.ToString();
                ChangeCompanyBox.Text = pr.Companies.Name;
                ChangeModelBlock.Text = pr.Model;
                ChangeCategoryBox.Text = pr.Categories.Name;
            }
        }

        private void Add_Image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG) | *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.*";
            Nullable<bool> result = file.ShowDialog();
            if (File.Exists(file.FileName))
            {
                Image = new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
                borderImage.Source = null;
                borderImage.Source = Image;
            }
        }
        private void Add_ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG) | *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.*";
            Nullable<bool> result = file.ShowDialog();
            if (File.Exists(file.FileName))
            {
                changeImage = new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
                borderImage2.Source = null;
                borderImage2.Source = changeImage;
            }
        }
    }
}