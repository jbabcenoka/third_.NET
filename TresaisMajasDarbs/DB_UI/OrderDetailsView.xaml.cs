using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TresaisMajasDarbs.Models;
using System.Linq;

namespace DB_UI
{
    /// <summary>
    /// Interaction logic for OrderDetailsView.xaml
    /// </summary>
    public partial class OrderDetailsView : Window
    {
        private DataSet ds = new DataSet();
        private DataTable dtOrdersOrderDetails = new DataTable("OrderOrderDetials");
        private int precesID;
        private int amount;
        private int pasutijumaID;
        private int OrderDetailIndex;

        public OrderDetailsView(DataSet dataset, int index)
        {
            InitializeComponent();
            ds = dataset;
            pasutijumaID = index;
            dtOrdersOrderDetails = ds.Tables["OrderOrderDetails"];
            AizpilditPrecesCbo();
            OrderDetailsGrid.ItemsSource = null;
            OrderDetailsGrid.ItemsSource = ds.Tables["OrderOrderDetails"].DefaultView;
            OrderDetailsGrid.CanUserSortColumns = false;
            OrderDetailsGrid.CanUserAddRows = false;
            OrderDetailsGrid.CanUserDeleteRows = false;
        }
        private void AizpilditPrecesCbo()
        {
            using (var db = new TestDBContext())
            {
                var allProducts = from product in db.Products
                                  select new { product.Name, product.Price  };
                foreach (var c in allProducts)
                {
                    var Prece = c.Name + " " + c.Price + " EUR";
                    this.cboProducts.Items.Add(Prece);
                }
            }
        }
        private void CreateOrderDetail(object sender, RoutedEventArgs e)
        {
            bool allCorrect = true;
            MessageBoxResult result = MessageBox.Show("Vai pievienot preci?\r\n", "Preces pievienošana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                //veidojam jauno rindiņu tabulā
                DataRow drOrderProducts = dtOrdersOrderDetails.NewRow();
                //pārbaudam un saglabājam ievadīto skaitu
                if (!String.IsNullOrEmpty(AmountTxt.Text))
                {
                    int amauntTemp;
                    bool success = Int32.TryParse(AmountTxt.Text, out amauntTemp);
                    if (success) { amount = amauntTemp; drOrderProducts["Amount"] = amount; }
                    if (cboProducts.SelectedItem == null) {
                        allCorrect = false;
                        MessageBox.Show("Ievadiet produktu!\r\n", "Preces pievienošana", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } else { 
                    allCorrect = false;
                    MessageBox.Show("Ievadiet skaitu!\r\n", "Preces pievienošana", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //pārbaudam un saglabājam izvēlēto preci
                if (allCorrect)
                {
                    using (var db = new TestDBContext())
                    {
                        var qryName = from prece in db.Products
                                      where String.Equals(prece.Name + " " + prece.Price + " EUR", cboProducts.SelectedItem.ToString())
                                      select new { prece.Id, prece.Name, prece.Price };

                        foreach (var c in qryName)
                        {
                            precesID = c.Id;
                            drOrderProducts["Name"] = c.Name;
                            drOrderProducts["Price"] = c.Price;
                        }
                    }
                    dtOrdersOrderDetails.Rows.Add(drOrderProducts);
                    SqlConnection cnNorthwind = new SqlConnection();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    try
                    {
                        //connection
                        cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                        cnNorthwind.Open();
                        //pievienojam SQLā

                        adapter.InsertCommand = new SqlCommand("INSERT INTO OrderDetails (ProductID, Amount, OrderID) VALUES(@ProductID, @Amount, @OrderID)", cnNorthwind);
                        adapter.InsertCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = (Int32)precesID;
                        adapter.InsertCommand.Parameters.Add("@Amount", SqlDbType.Int).Value = (Int32)amount;
                        adapter.InsertCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = (Int32)pasutijumaID;
                        adapter.InsertCommand.ExecuteNonQuery();
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
                            cnNorthwind.Dispose();
                            cnNorthwind = null;
                            ds.AcceptChanges();
                            MessageBox.Show("Veiksmigi pievienoti produkti!", "Preces pievienošana", MessageBoxButton.OK);
                        }
                    }

                }
            }
        }
        private void UpdateOrderDetail(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Vai reģēt pasūtījuma detaļu?\r\n", "Pasūtījuma rediģēšana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                if (OrderDetailsGrid.SelectedIndex == -1) { MessageBox.Show("Noradiet pasūtījuma detaļu - izvēlēties rindiņu", "Pasūtījuma detaļas rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
                else
                {
                    if (String.IsNullOrEmpty(AmountTxt.Text) && cboProducts.SelectedIndex < 1)
                    {
                        MessageBox.Show("Ievadiet jauno skaitu un/vai produktu", "Produkta rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        //sākuma dati 
                        string productName = (String)dtOrdersOrderDetails.Rows[OrderDetailsGrid.SelectedIndex]["Name"];
                        decimal productPrice = (Decimal)dtOrdersOrderDetails.Rows[OrderDetailsGrid.SelectedIndex]["Price"];
                        amount = (Int32)dtOrdersOrderDetails.Rows[OrderDetailsGrid.SelectedIndex]["Amount"];

                        //sākumā uzzināsim PrecesID, kas ir attiecīgajam OrderDetail
                        using (var db = new TestDBContext())
                        {
                            var qryName = from prece in db.Products
                                          where String.Equals(prece.Name + prece.Price, productName + productPrice)
                                          select prece.Id;
                            foreach (var c in qryName) { precesID = c; }
                        }
                        //REDIĢĒJAM
                        DataRow drOrderDetail = dtOrdersOrderDetails.Rows[OrderDetailsGrid.SelectedIndex];
                        drOrderDetail.BeginEdit();
                        //ja ir ievadīts jauns DAUDZUMS - mainām -> pārbaudam un saglabājam ievadīto skaitu
                        if (!String.IsNullOrEmpty(AmountTxt.Text))
                        {
                            int amauntTemp;
                            bool success = Int32.TryParse(AmountTxt.Text, out amauntTemp);
                            if (success) { amount = amauntTemp; drOrderDetail["Amount"] = amount; }
                        }
                        //ja ir ievadīts jauna PRECE - mainām -> pārbaudam un saglabājam ievadīto preci tabulā
                        if (cboProducts.SelectedIndex > 0)
                        {
                            using (var db = new TestDBContext())
                            {
                                var qryName = from prece in db.Products
                                              where String.Equals(prece.Name + " " + prece.Price + " EUR", cboProducts.SelectedItem.ToString())
                                              select new { prece.Id, prece.Name, prece.Price };

                                foreach (var c in qryName)
                                {
                                    precesID = c.Id;
                                    drOrderDetail["Name"] = c.Name;
                                    drOrderDetail["Price"] = c.Price;
                                }
                            }
                        }

                        drOrderDetail.EndEdit();
                        
                        //meklējām izvēlēto detaļas indeksu, lai būtu atslēga
                        using (var db = new TestDBContext())
                        {
                            var qryName = from detail in db.OrderDetails
                                          where detail.OrderId == pasutijumaID
                                          select detail.Id;
                            int i = 0;
                            foreach (var c in qryName)
                            {

                                if (i == OrderDetailsGrid.SelectedIndex)
                                {
                                    OrderDetailIndex = c;
                                    break;
                                }
                                i++;
                            }
                        }
                        SqlConnection cnNorthwind = new SqlConnection();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        try
                        {
                            //connection
                            cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                            cnNorthwind.Open();
                            //saglabājam SQLā
                            adapter.UpdateCommand = new SqlCommand("UPDATE OrderDetails SET ProductID = @ProductID, Amount = @Amount WHERE ID = @orderDetalasID", cnNorthwind);
                            adapter.UpdateCommand.Parameters.Add("@orderDetalasID", SqlDbType.Int).Value = OrderDetailIndex;
                            adapter.UpdateCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = precesID;
                            adapter.UpdateCommand.Parameters.Add("@Amount", SqlDbType.Int).Value = amount;
                            adapter.UpdateCommand.ExecuteNonQuery();
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
                                ds.AcceptChanges();
                                cnNorthwind.Close();
                                cnNorthwind.Dispose();
                                cnNorthwind = null;
                                MessageBox.Show("Veiksmigi rediģēts pasūtījums!", "Pasūtījuma rediģēšana", MessageBoxButton.OK);
                            }
                        }
                    }
                }
            }

        }
        private void DeleteOrderDetail(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Vai dzēst pasūtījuma detaļu?\r\n", "Pasūtījuma detaļas dzēšana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                if (OrderDetailsGrid.SelectedIndex == -1) { MessageBox.Show("Noradiet pasūtījuma detaļu - izvēlēties rindiņu", "Pasūtījuma detaļas rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
                else
                {
                    //meklējām izvēlēto detaļas indeksu, lai būtu atslēga
                    using (var db = new TestDBContext())
                    {
                        var qryName = from detail in db.OrderDetails
                                      where detail.OrderId == pasutijumaID
                                      select detail.Id;
                        int i = 0;
                        foreach (var c in qryName)
                        {

                            if (i == OrderDetailsGrid.SelectedIndex)
                            {
                                OrderDetailIndex = c;
                                break;
                            }
                            i++;
                        }
                    }

                    //dzēst no lietotāja tabulas
                    DataRow drOrderDetail = dtOrdersOrderDetails.Rows[OrderDetailsGrid.SelectedIndex];
                    dtOrdersOrderDetails.Rows.Remove(drOrderDetail);

                    //dzēst datubāzē
                    SqlConnection cnNorthwind = new SqlConnection();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    try {
                        //connection
                        cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                        cnNorthwind.Open();
                        adapter.DeleteCommand = new SqlCommand("DELETE FROM OrderDetails WHERE ID = @OrderDetailID", cnNorthwind);
                        adapter.DeleteCommand.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailIndex;
                        adapter.DeleteCommand.ExecuteNonQuery();
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
                            cnNorthwind.Dispose();
                            cnNorthwind = null;
                            ds.AcceptChanges();
                            MessageBox.Show("Veiksmigi dzēsta pasūtījuma detaļa!", "Pasūtījuma detaļas", MessageBoxButton.OK);
                        }
                    }
                }
            }
        }
    }
}
