using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

namespace DB_UI
{
    /// <summary>
    /// Interaction logic for JaunsPasutijums.xaml
    /// </summary>

    public partial class JaunsPasutijums : Window
    {
        private DataTable dt = new DataTable();
        private DataSet ds = new DataSet();
        

        private string number;
        private int CustomerID;
        private DateTime date1 = new DateTime();
        public JaunsPasutijums(DataTable datatable, DataSet dataset)
        {
            InitializeComponent();
            dt = datatable;
            ds = dataset;
            AizpilditKlientus();
            AizpilditStates();
        }
        private void AizpilditKlientus()
        {
                using (var db = new TestDBContext())
                {
                    var allCustomers = from cust in db.Customers
                                       orderby cust.Name
                                       select new { cust.Name, cust.Surname, cust.Email };
                    foreach (var c in allCustomers)
                    {
                        var Pasutitajs = c.Name  + " " + c.Surname + " " + c.Email;
                        this.cboPasutitajs.Items.Add(Pasutitajs);
                    }
                }
        }
        private void AizpilditStates()
        {
            cboState.Items.Insert(0, "New");
            cboState.Items.Insert(1, "Completed");
            cboState.Items.Insert(2, "Canceled");
            cboState.Items.Insert(3, "AwaitingPayment");
            cboState.Items.Insert(4, "Pending");
            cboState.Items.Insert(5, "AwaitingPickup");
        }
        public void CreateOrder(object sender, RoutedEventArgs e)
        {
               
                MessageBoxResult result = MessageBox.Show("Vai izveidot pasūtījumu?\r\n", "Pasūtījuma pievienošana", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    if (String.IsNullOrEmpty(PasutNum.Text) || cboState.SelectedItem == null || !PasutDate.SelectedDate.HasValue
                        || cboPasutitajs.SelectedIndex<=1) 
                    {
                        MessageBox.Show("Aizpildiet visus laukus!\r\n", "Pasūtījuma pievienošana", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        bool allCorrect = true;
                        //pārbaudam vai datu bāzē ir šis pasūtījuma numurs , jau ir -> kļūda
                        using (var db = new TestDBContext())
                        {
                            var PasutNumber = from pasut in db.Orders
                                              where pasut.Number == PasutNum.Text
                                              select pasut.Id;
                            if (PasutNumber.Any()) { allCorrect = false; }
                        }
                    if (!allCorrect) { MessageBox.Show("Šis pasūtījuma numurds datu bāzē jau eksistē!", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
                    else
                    {
                            //veidojam jaunu rindiņu Order tabulā 
                            DataRow drNewOrder = dt.NewRow();
                            number = PasutNum.Text;
                            drNewOrder["Number"] = PasutNum.Text;
                            var state = cboState.SelectedItem; ;
                            drNewOrder["State"] = state;
                            DateTime.TryParse(PasutDate.Text, out date1);
                            drNewOrder["OrderDate"] = date1;
                            if (cboPasutitajs.SelectedIndex > 0)
                            {
                                using (var db = new TestDBContext())
                                {
                                    var qryName = from cust in db.Customers
                                                  where String.Equals(cust.Name + " " + cust.Surname + " " + cust.Email, cboPasutitajs.SelectedItem.ToString())
                                                  select new { cust.Id, cust.Name, cust.Surname, cust.Email };

                                    foreach (var c in qryName)
                                    {
                                        CustomerID = c.Id;
                                        drNewOrder["Name"] = c.Name;
                                        drNewOrder["Surname"] = c.Surname;
                                        drNewOrder["Email"] = c.Email;
                                    }
                                }
                            }
                            //saglabājam tabulā
                            dt.Rows.Add(drNewOrder);

                            SqlConnection cnNorthwind = new SqlConnection();
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            //saglabājam SQLā
                            try
                            {
                                //connection
                                cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                                cnNorthwind.Open();
                                adapter.InsertCommand = new SqlCommand("INSERT INTO Orders (Number,State,OrderDate,CustomerID) VALUES(@number, @state, @date, @CustomerID)", cnNorthwind);
                                adapter.InsertCommand.Parameters.Add("@number", SqlDbType.NVarChar).Value = number;
                                adapter.InsertCommand.Parameters.Add("@state", SqlDbType.Int).Value = (Int32)cboState.SelectedIndex;
                                adapter.InsertCommand.Parameters.Add("@date", SqlDbType.DateTime2).Value = date1;
                                adapter.InsertCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
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
                                    dt.AcceptChanges();
                                    MessageBox.Show("Veiksmigi izveidots jauns pasūtījums!", "Pasūtījuma izveidošana", MessageBoxButton.OK);
                                    this.Close();
                                }
                            }
                        }

                    }
                }
             
         }
    }
}
