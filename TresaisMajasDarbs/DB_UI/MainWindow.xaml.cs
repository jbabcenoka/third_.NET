using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TresaisMajasDarbs.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Data.Entity.Core;

//Autors: Jevgenija Babčenoka, apl.numurs jb19045.
//Izmantoju Estudijās doto datu bāzi.
//Connection string tika norādits App.config failā.
//Izmantoju metodi "Database first"
//Izmantoju ADO.NET, datus attēloju DataGrid tabulās


namespace DB_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DataSet ds = new DataSet();
        private DataTable dtOrders = new DataTable("Orders");
        private DataTable dtProducts = new DataTable("Products");
       

        private string productName;
        private decimal price;
        int precesID;
        int pasutijumaID;

        public MainWindow()
        {
            InitializeComponent();
            CreateOrderTable(); //aizpilda DataTable tabulas + 
            CreateProductTable();
        }

        //Metodes Order / OrderDetail tabulai
        private void CreateOrderTable()
        {
            SqlConnection cnNorthwind = new SqlConnection();
            SqlDataAdapter adapter = new SqlDataAdapter();

            DataColumn column1 = new DataColumn("Number", typeof(String));
            DataColumn column2 = new DataColumn("State", typeof(String));
            DataColumn column3 = new DataColumn("OrderDate", typeof(DateTime));
            DataColumn column4 = new DataColumn("Name", typeof(String));
            DataColumn column5 = new DataColumn("Surname", typeof(String));
            DataColumn column6 = new DataColumn("Email", typeof(String));
            try
            {
                //connection
                cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                cnNorthwind.Open();
                //query
                adapter.SelectCommand = new SqlCommand("SELECT Number, State, OrderDate, Name, Surname, Email FROM Orders, Customer WHERE CustomerID = Customer.ID", cnNorthwind);
                //pievienojam kolonnas tabulā Order
                
                dtOrders.Columns.Add(column1);
                dtOrders.Columns.Add(column2);
                dtOrders.Columns.Add(column3);
                dtOrders.Columns.Add(column4);
                dtOrders.Columns.Add(column5);
                dtOrders.Columns.Add(column6);

                ds.Tables.Add(dtOrders);  //pievienojam Setā
                ds.Tables[0].BeginLoadData();
                adapter.Fill(dtOrders);
                foreach (DataRow dr in dtOrders.Rows)  //katrā rindiņā rediģējam state nosukumus
                {
                    if (String.Equals(dr["State"], "0")) { dr["State"] = "New"; }
                    if (String.Equals(dr["State"], "1")) { dr["State"] = "Completed"; }
                    if (String.Equals(dr["State"], "2")) { dr["State"] = "Canceled"; }
                    if (String.Equals(dr["State"], "3")) { dr["State"] = "AwaitingPayment"; }
                    if (String.Equals(dr["State"], "4")) { dr["State"] = "Pending"; }
                    if (String.Equals(dr["State"], "5")) { dr["State"] = "AwaitingPickup"; }
                }
                ds.Tables[0].EndLoadData();
                ds.AcceptChanges();
            }
            catch (System.Exception Xcp)
            {
                MessageBox.Show(Xcp.Message);
            }
            finally
            {
                if (cnNorthwind.State == ConnectionState.Open)
                {   
                    OrderCustomerGrid.ItemsSource = dtOrders.DefaultView;
                    OrderCustomerGrid.CanUserSortColumns = false;
                    OrderCustomerGrid.CanUserAddRows = false;
                    OrderCustomerGrid.CanUserDeleteRows = false;
                    cnNorthwind.Close();
                    cnNorthwind.Dispose();
                    cnNorthwind = null;
                }
            }
        }
        public void CreateOrder(object sender, RoutedEventArgs e)
        {
            JaunsPasutijums neworder = new JaunsPasutijums(dtOrders, ds);
            neworder.ShowDialog();
        }
        public void ShowDetails(object sender, RoutedEventArgs e)
        {
            if (OrderCustomerGrid.SelectedIndex == -1) { MessageBox.Show("Noradiet pasūtījumu - izvēlēties rindiņu", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
            else
            {
                //sagalabājam pasūtījuma numuru
                var selected_numurs = (String)dtOrders.Rows[OrderCustomerGrid.SelectedIndex]["Number"];

                //meklējam pasūtījuma indexu 
                using (var db = new TestDBContext())
                {
                    var PasutNumber = from pasut in db.Orders
                                      where String.Equals(selected_numurs, (String)pasut.Number)
                                      select pasut.Id;
                    foreach (var c in PasutNumber) { pasutijumaID = c; }
                }
                //aizpildam tabulu OrderDetails attiecīgajam pasūtījumam un atvēram logu 
                SqlConnection cnNorthwind = new SqlConnection();
                SqlDataAdapter adapter1 = new SqlDataAdapter();
                try
                {
                    //connection
                    cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                    cnNorthwind.Open();
                    adapter1.SelectCommand = new SqlCommand("SELECT Name, Price, Amount FROM OrderDetails, Product WHERE ProductID=Product.ID AND OrderDetails.OrderID = " + pasutijumaID, cnNorthwind);
                    DataTable dtOrderOrderDetails = new DataTable("OrderOrderDetails");
                    if (ds.Tables["OrderOrderDetails"] != null) { dtOrderOrderDetails.Clear(); ds.Tables.Remove("OrderOrderDetails"); }
                    ds.Tables.Add(dtOrderOrderDetails);
                    ds.Tables["OrderOrderDetails"].BeginLoadData();
                    adapter1.Fill(ds, "OrderOrderDetails");
                    ds.Tables["OrderOrderDetails"].EndLoadData();
                }
                catch (System.InvalidOperationException XcpInvOP)
                {
                    MessageBox.Show(XcpInvOP.Message);
                }
                catch (System.Exception Xcp)
                {
                    MessageBox.Show(Xcp.Message);
                }
                finally
                {
                    if (cnNorthwind.State == ConnectionState.Open)
                    {
                        cnNorthwind.Close();
                        //AizpilditOrderDetails(order_index);
                        cnNorthwind.Dispose();
                        cnNorthwind = null;
                        OrderDetailsView detalas = new OrderDetailsView(ds, pasutijumaID);
                        detalas.ShowDialog();
                    }
                }
            }
        }
        private void UpdateOrder(object sender, RoutedEventArgs e)
        {
            int order_index = OrderCustomerGrid.SelectedIndex; //dabūjam izvēlēto pasūtījuma indexu
            if (order_index == -1) { MessageBox.Show("Noradiet pasūtījumu - izvēlēties rindiņu", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
            else {
                UpdateOrder updatepage = new UpdateOrder(ds, dtOrders, order_index);  //atvēram rediģēšanas lapu
                updatepage.ShowDialog();
            }
        }
        private void DeleteOrder(object sender, RoutedEventArgs e)
        {
            if (OrderCustomerGrid.SelectedIndex == -1) { MessageBox.Show("Noradiet pasūtījumu - izvēlēties rindiņu", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
            else
            {
                //atrodam rindiņu, kuru izvēlējamies dzēst 
                DataRow drOrder = dtOrders.Rows[OrderCustomerGrid.SelectedIndex];
                var selected_numurs = drOrder["Number"].ToString();
                //meklējam pasūtījuma numuru 
                using (var db = new TestDBContext())
                {
                    var PasutNumber = from pasut in db.Orders
                                      where String.Equals(selected_numurs, (String)pasut.Number)
                                      select pasut.Id;
                    foreach (var c in PasutNumber) { pasutijumaID = c; }
                }
                //dzēst no lietotāja tabulas
                dtOrders.Rows.Remove(drOrder);

                //dzēst datu bāzē
                SqlConnection cnNorthwind = new SqlConnection();
                SqlDataAdapter adapter = new SqlDataAdapter();
                try
                {
                    //connection
                    cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                    cnNorthwind.Open();
                    adapter.DeleteCommand = new SqlCommand("DELETE FROM OrderDetails WHERE OrderID = @pasutijumaID", cnNorthwind);
                    adapter.DeleteCommand.Parameters.Add("@pasutijumaID", SqlDbType.Int).Value = pasutijumaID;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    adapter.DeleteCommand = new SqlCommand("DELETE FROM Orders WHERE Number = @selected_numurs", cnNorthwind);
                    adapter.DeleteCommand.Parameters.Add("@selected_numurs", SqlDbType.NVarChar).Value = selected_numurs;
                    adapter.DeleteCommand.ExecuteNonQuery();
                }
                catch (System.InvalidOperationException XcplnvOP)
                {
                    MessageBox.Show(XcplnvOP.Message);
                }
                catch (System.Exception Xcp)
                {
                    MessageBox.Show(Xcp.Message);
                }
                finally
                {
                    if (cnNorthwind.State == ConnectionState.Open)
                    {

                        cnNorthwind.Close();
                        ds.AcceptChanges();
                        MessageBox.Show("Veiksmigi dzēsts pasūtījums un tam atbilstošas pasūtījuma detaļas!", "Pasūtījuma dzēšana", MessageBoxButton.OK);
                        cnNorthwind.Dispose();
                        cnNorthwind = null;
                    }
                }
            }
        }


        //Metodes Product tabulai:
        private void CreateProductTable()
        {
            SqlConnection cnNorthwind = new SqlConnection();
            SqlDataAdapter adapter = new SqlDataAdapter();
            try
            {
                //connection
                cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                cnNorthwind.Open();
                //query
                adapter.SelectCommand = new SqlCommand("SELECT Name, Price FROM Product", cnNorthwind);
                ds.Tables.Add(dtProducts);  //pievienojam Setā
                ds.Tables[1].BeginLoadData();
                adapter.Fill(dtProducts);
                ds.Tables[1].EndLoadData();
            }
            catch (System.InvalidOperationException XcplnvOP)
            {
                MessageBox.Show(XcplnvOP.Message);
            }
            catch (System.Exception Xcp)
            {
                MessageBox.Show(Xcp.Message);
            }
            finally
            {
                if (cnNorthwind.State == ConnectionState.Open)
                {
                    cnNorthwind.Close();
                    cnNorthwind.Dispose();
                    cnNorthwind = null;
                    ProductGrid.ItemsSource = ds.Tables[1].DefaultView;
                    ProductGrid.CanUserSortColumns = false;
                    ProductGrid.CanUserAddRows = false;
                    ProductGrid.CanUserDeleteRows = false;
                }
            }
        }
        private void CreateProduct(object sender, RoutedEventArgs e)
        {
            bool allCorrect = true;
            
            MessageBoxResult result = MessageBox.Show("Vai pievienot preci?\r\n", "Preces pievienošana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                //veidojam jauno rindiņu tabulā
                DataRow drProduct = dtProducts.NewRow();

                //pārbaudam un saglabājam ievadīto skaitu
                if (!String.IsNullOrEmpty(ProductNameTxt.Text))
                {
                    productName = ProductNameTxt.Text;
                    drProduct["Name"] = productName;
                }
                else
                {
                    allCorrect = false;
                    MessageBox.Show("Ievadiet nosaukumu!\r\n", "Preces pievienošana", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                if (allCorrect)
                {   
                    //pārbaudaudam un saglabājam produkta cenu
                    if (!String.IsNullOrEmpty(ProductPriceTxt.Text))
                    {   
                        decimal priceTemp;
                        bool success = Decimal.TryParse(ProductPriceTxt.Text, out priceTemp);
                        //ja ievadītie lauki ir pareizi norāditi, saglabājam lietotāja tabulā
                        if (success) { 
                            price = priceTemp; drProduct["Price"] = price; 
                            dtProducts.Rows.Add(drProduct);

                            //saglabājam SQLā
                            if (ds.HasChanges(DataRowState.Added))
                            {
                                SqlConnection cnNorthwind = new SqlConnection();
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                try
                                {
                                    //connection
                                    cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                                    cnNorthwind.Open();
                                    adapter.InsertCommand = new SqlCommand("INSERT INTO Product (Name,Price) OUTPUT INSERTED.ID VALUES(@Name,@Price)", cnNorthwind);
                                    adapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar).Value = productName;
                                    adapter.InsertCommand.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                catch (System.InvalidOperationException XcplnvOP)
                                {
                                    MessageBox.Show(XcplnvOP.Message);
                                }
                                catch (System.Exception Xcp)
                                {
                                    MessageBox.Show(Xcp.Message);
                                }
                                finally
                                {
                                    if (cnNorthwind.State == ConnectionState.Open)
                                    {
                                        cnNorthwind.Close();
                                        cnNorthwind.Dispose();
                                        cnNorthwind = null;
                                        ds.AcceptChanges();
                                        MessageBox.Show("Veiksmigi pievienots produkts!", "Produkta pieveinošana", MessageBoxButton.OK);
                                    }
                                }

                            }
                        }
                    }
                    else { MessageBox.Show("Ievadiet cenu!\r\n", "Preces pievienošana", MessageBoxButton.OKCancel, MessageBoxImage.Error);}
                }
            } 
        }
        private void UpdateProduct(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Vai reģēt preci?\r\n", "Preces rediģēšana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                if (ProductGrid.SelectedIndex == -1) { MessageBox.Show("Noradiet produktu - izvēlēties rindiņu", "Produkta rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
                else
                {
                    if (String.IsNullOrEmpty(ProductNameTxt.Text) && String.IsNullOrEmpty(ProductPriceTxt.Text))
                    {
                        MessageBox.Show("Ievadiet jauno nosaukumu un/vai cenu", "Produkta rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        //saglabājam sākotnējas vērtības
                        string productName = (String)dtProducts.Rows[ProductGrid.SelectedIndex]["Name"];
                        decimal productPrice = (Decimal)dtProducts.Rows[ProductGrid.SelectedIndex]["Price"];
                        //meklējam produkta indexu, lai izmantotu Update komandā
                       
                            using (var db = new TestDBContext())
                            {
                                var qryName = from prod in db.Products
                                              where prod.Name == productName && prod.Price == productPrice
                                              select prod.Id;
                                foreach (var c in qryName) { precesID = c; }
                            }
                        DataRow drProduct = dtProducts.Rows[ProductGrid.SelectedIndex];
                        drProduct.BeginEdit();

                        bool success = true;
                        //ja lietotājs ievada produkta nosaukumu -> pārbaudam un saglabājam ievadīto nosaukumu
                        if (!String.IsNullOrEmpty(ProductNameTxt.Text))
                        {
                            productName = ProductNameTxt.Text;
                            drProduct["Name"] = productName;
                        }
                        //ja lietotājs ievada produkta cenu -> pārbaudam un saglabājam ievadīto cenu
                        if (!String.IsNullOrEmpty(ProductPriceTxt.Text))
                        {
                            decimal priceTemp;
                            success = Decimal.TryParse(ProductPriceTxt.Text, out priceTemp);
                            if (success) { productPrice = priceTemp; drProduct["Price"] = productPrice; }
                            else
                            {
                                MessageBox.Show("Nepareizi ievadīta cena", "Produkta rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error);
                                success = false;
                            }
                        }
                        drProduct.EndEdit();
                        if (success)
                        {
                            SqlConnection cnNorthwind = new SqlConnection();
                            try
                            {
                                //connection
                                cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                                cnNorthwind.Open();
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                adapter.UpdateCommand = new SqlCommand("UPDATE Product SET Name = @Name, Price = @Price WHERE ID = @ProductID", cnNorthwind);
                                adapter.UpdateCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = precesID;
                                adapter.UpdateCommand.Parameters.Add("@Name", SqlDbType.NVarChar).Value = productName;
                                adapter.UpdateCommand.Parameters.Add("@Price", SqlDbType.Decimal).Value = productPrice;
                                adapter.UpdateCommand.ExecuteNonQuery();
                            }
                            catch (System.InvalidOperationException XcplnvOP)
                            {
                                MessageBox.Show(XcplnvOP.Message);
                            }
                            catch (System.Exception Xcp)
                            {
                                MessageBox.Show(Xcp.Message);
                            }
                            finally
                            {
                                if (cnNorthwind.State == ConnectionState.Open)
                                {
                                    cnNorthwind.Close();
                                    cnNorthwind.Dispose();
                                    cnNorthwind = null;
                                    ds.AcceptChanges();
                                    MessageBox.Show("Veiksmigi rediģēts produkts!", "Produkta rediģēšana", MessageBoxButton.OK);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void DeleteProduct(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Vai dzēst preci? Visas pasūtījuma detaļas, kas ir saistītas ar šo produktu, tiks dzēsti. \r\n", "Preces dzēšana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                if (ProductGrid.SelectedIndex == -1) { MessageBox.Show("Noradiet produktu - izvēlēties rindiņu", "Produkta dzēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
                else
                { 
                    //uzzinājam sākotnējas vērtības
                    string productName = (String)dtProducts.Rows[ProductGrid.SelectedIndex]["Name"];
                    decimal productPrice = (Decimal)dtProducts.Rows[ProductGrid.SelectedIndex]["Price"];

                    //meklējam produkta indexu, lai izmantotu Update komandā
                    using (var db = new TestDBContext())
                    {
                        var qryName = from prod in db.Products
                                      where prod.Name == productName && prod.Price == productPrice
                                      select prod.Id;
                        foreach (var c in qryName) { precesID = c; }
                    }

                    //dzēst no lietotāja tabulas
                    DataRow drProduct = dtProducts.Rows[ProductGrid.SelectedIndex];
                    dtProducts.Rows.Remove(drProduct);

                    //dzēst datubāzē
                    SqlConnection cnNorthwind = new SqlConnection();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    try
                    {
                        //connection
                        cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                        cnNorthwind.Open();

                        //dzēst no pasūtījuma detaļas
                        adapter.DeleteCommand = new SqlCommand("DELETE FROM OrderDetails WHERE ProductID = @ProductID", cnNorthwind);
                        adapter.DeleteCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = precesID;
                        adapter.DeleteCommand.ExecuteNonQuery();
                        //dzēst produktu
                        adapter.DeleteCommand = new SqlCommand("DELETE FROM Product WHERE ID = @ProductID", cnNorthwind);
                        adapter.DeleteCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = precesID;
                        adapter.DeleteCommand.ExecuteNonQuery();
                        
                    }
                    catch (System.InvalidOperationException XcplnvOP)
                    {
                        MessageBox.Show(XcplnvOP.Message);
                    }
                    catch (System.Exception Xcp)
                    {
                        MessageBox.Show(Xcp.Message);
                    }
                    finally
                    {
                        if (cnNorthwind.State == ConnectionState.Open)
                        {
                            cnNorthwind.Close();
                            cnNorthwind.Dispose();
                            cnNorthwind = null;
                            ds.AcceptChanges();
                            MessageBox.Show("Veiksmigi dzēsts produkts!", "Pasūtījuma detaļas", MessageBoxButton.OK);
                        }
                    }
                }
            }
        }
        
    }
}
