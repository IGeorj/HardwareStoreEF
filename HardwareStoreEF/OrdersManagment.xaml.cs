using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Reflection;
using System;
using System.Globalization;
using Microsoft.Win32;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;


namespace HardwareStoreEF
{
    public partial class OrdersManagment : System.Windows.Window
    {
        public OrdersManagment()
        {
            InitializeComponent();
            Update();
        }
        private void FindAndReplace(Word.Application wordApp, object ToFindText, object replaceWithText)
        {
            object matchCase = true;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundLike = false;
            object nmatchAllforms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiactitics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;

            wordApp.Selection.Find.Execute(ref ToFindText,
                ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundLike,
                ref nmatchAllforms, ref forward,
                ref wrap, ref format, ref replaceWithText,
                ref replace, ref matchKashida,
                ref matchDiactitics, ref matchAlefHamza,
                ref matchControl);
        }
        private bool CreateWordDocument(object filename, object SaveAs, string category, string model, string amount, string price, string allPrice)
        {
            Word.Application wordApp = new Word.Application();
            object missing = Missing.Value;
            Word.Document myWordDoc = null;

            if (File.Exists((string)filename))
            {
                object readOnly = false;
                object isVisible = false;
                wordApp.Visible = false;

                myWordDoc = wordApp.Documents.Open(ref filename, ref missing, ref readOnly,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing, ref missing);
                myWordDoc.Activate();

                this.FindAndReplace(wordApp, "<Category>", category);
                this.FindAndReplace(wordApp, "<Model>", model);
                this.FindAndReplace(wordApp, "<Amount>", amount);
                this.FindAndReplace(wordApp, "<Price>", price + " руб.");
                this.FindAndReplace(wordApp, "<AllPrice>", allPrice + " руб.");
                this.FindAndReplace(wordApp, "<Day>", DateTime.Now.Day.ToString());
                this.FindAndReplace(wordApp, "<Month>", DateTime.Now.ToString("MMM", new CultureInfo("ru-RU")));
                this.FindAndReplace(wordApp, "<Year>", DateTime.Now.Year.ToString());

            }
            else
            {
                MessageBox.Show("File not Found!");
                return false;
            }

            myWordDoc.SaveAs2(ref SaveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing);

            myWordDoc.Close();
            wordApp.Quit();
            MessageBox.Show("File Created!");
            return true;
        }
        public void Update()
        {
            using (DBContext db = new DBContext())
            {
                OrdersGrid.ItemsSource = (from ord in db.Orders
                                          join user in db.Users on ord.UserID equals user.UserID
                                          join prod in db.Products on ord.ProductID equals prod.ProductID
                                          select new
                                          {
                                              ord.Amount,
                                              Address = ord.Users.Address,
                                              Model = ord.Products.Model,
                                              Company = prod.Companies.Name,
                                              FirstName = ord.Users.FirstName,
                                              LastName = ord.Users.LastName
                                          }).ToList();
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            object SelectedProduct = OrdersGrid.SelectedItem;
            string company = (OrdersGrid.SelectedCells[0].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string model = (OrdersGrid.SelectedCells[1].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string amount = (OrdersGrid.SelectedCells[2].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            string Address = (OrdersGrid.SelectedCells[3].Column.GetCellContent(SelectedProduct) as TextBlock).Text;
            using (DBContext db = new DBContext())
            {
                Products pr = db.Products.FirstOrDefault(o => (o.Model == model && o.Companies.Name == company));
                string allPrice = (pr.Price * int.Parse(amount)).ToString();

                Users us = db.Users.FirstOrDefault(n => n.Address == Address);
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Filter = "Word Documents (*.doc)|*.doc";
                savefile.FileName = "NewCheck.doc";
                if (savefile.ShowDialog() == false)
                    return;
                string filename = savefile.FileName;
                if(CreateWordDocument(AppDomain.CurrentDomain.BaseDirectory + @"\CheckOut.doc",
                    filename,
                    pr.Categories.Name, model, amount, pr.Price.ToString(), allPrice) == false)
                return;
                db.Orders.Remove(db.Orders.FirstOrDefault(s => s.ProductID == pr.ProductID && s.UserID == us.UserID));
                db.SaveChanges();
            }
            Update();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = false;
            Excel.Workbook workbook = excel.Workbooks.Add(Missing.Value);
            workbook.Activate();
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];

            for (int j = 0; j < OrdersGrid.Columns.Count; j++) 
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = OrdersGrid.Columns[j].Header;
            }
            for (int i = 0; i < OrdersGrid.Columns.Count - 1; i++)
            {
                for (int j = 0; j < OrdersGrid.Items.Count; j++)
                {
                    TextBlock b = OrdersGrid.Columns[i].GetCellContent(OrdersGrid.Items[j]) as TextBlock;
                    if (b.Text == null)
                        break;
                    Excel.Range myRange = (Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            savefile.FileName = $"AllOrders_{DateTime.Now.Day.ToString()}.{DateTime.Now.Month}.{DateTime.Now.Year}_{DateTime.Now.Hour}.{DateTime.Now.Minute}.xlsx";
            if (savefile.ShowDialog() == false)
                return;
            string filename = savefile.FileName;
            excel.DisplayAlerts = false;
            workbook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);
            workbook.Close();
            excel.Quit();
            MessageBox.Show("File Created!");
        }
    }
}