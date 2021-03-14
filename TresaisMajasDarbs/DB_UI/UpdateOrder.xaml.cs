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
    /// Interaction logic for UpdateOrder.xaml
    /// </summary>

    public partial class UpdateOrder : Window
    {
        private DataTable dt = new DataTable("Orders");
        private DataSet ds = new DataSet();
        private int pasutijumaIndex;
        private string number;
        private string CustomerName;
        private string CustomerSurname;
        private string CustomerEmail;
        private int stateIndex;
        private int CustomerID;
        private int pasutijumaID;
        private DateTime date1 = new DateTime();
        public UpdateOrder(DataSet dataset, DataTable datatable, int index)
        {
            InitializeComponent();
            dt = datatable;
            ds = dataset;
            pasutijumaIndex = index;
            //cbo aizpildīšana
            cboCustomersFill(); 
            cboStatesFill();
            cboUpdateChoiseFill();
            //parādīt sakotnējas vērtības
            SakotnejasVertibas();
        }
        private void cboCustomersFill()
        {
            using (var db = new TestDBContext())
            {
                var allCustomers = from cust in db.Customers
                                   orderby cust.Name
                                   select new { cust.Name, cust.Surname, cust.Email };
                foreach (var c in allCustomers)
                {
                    var Pasutitajs = c.Name + " " + c.Surname + " " + c.Email;
                    this.cboPasutitajs.Items.Add(Pasutitajs);
                }
            }
        }
        private void cboStatesFill()
        {
            cboStavoklis.Items.Insert(0, "New");
            cboStavoklis.Items.Insert(1, "Completed");
            cboStavoklis.Items.Insert(2, "Canceled");
            cboStavoklis.Items.Insert(3, "AwaitingPayment");
            cboStavoklis.Items.Insert(4, "Pending");
            cboStavoklis.Items.Insert(5, "AwaitingPickup");
        }
        private void cboUpdateChoiseFill()
        {
            cboUpdateChoise.Items.Insert(0, "Pasūtījuma numuru");
            cboUpdateChoise.Items.Insert(1, "Stāvokļu");
            cboUpdateChoise.Items.Insert(2, "Datumu");
            cboUpdateChoise.Items.Insert(3, "Pasūtītāju");
        }
        private void SakotnejasVertibas()
        {

            number = (String)dt.Rows[pasutijumaIndex]["Number"];
            //mēklējam stāvokļu
            if (String.Equals(dt.Rows[pasutijumaIndex]["State"],"New")) { stateIndex = 0; }
            if (String.Equals(dt.Rows[pasutijumaIndex]["State"], "Completed")) { stateIndex = 1; }
            if (String.Equals(dt.Rows[pasutijumaIndex]["State"], "Canceled")) { stateIndex = 2; }
            if (String.Equals(dt.Rows[pasutijumaIndex]["State"], "AwaitingPayment")) { stateIndex = 3; }
            if (String.Equals(dt.Rows[pasutijumaIndex]["State"], "Pending")) { stateIndex = 4; }
            if (String.Equals(dt.Rows[pasutijumaIndex]["State"], "AwaitingPickup")) { stateIndex = 5; }

            date1 = (DateTime)dt.Rows[pasutijumaIndex]["OrderDate"];

            CustomerName = (String)dt.Rows[pasutijumaIndex]["Name"];
            CustomerSurname = (String)dt.Rows[pasutijumaIndex]["Surname"];
            CustomerEmail = (String)dt.Rows[pasutijumaIndex]["Email"];


            //meklējam pasūtītāja index, lai izmantotu Update komandā
            using (var db = new TestDBContext())
            {
                var qryName = from cust in db.Customers
                              where String.Equals(cust.Name + " " + cust.Surname + " " + cust.Email, CustomerName + " " + CustomerSurname + " " + CustomerEmail)
                              select new { cust.Id, cust.Name, cust.Surname, cust.Email };
                foreach (var c in qryName)
                {
                    CustomerID = c.Id;
                    CustomerName = c.Name;
                    CustomerSurname = c.Surname;
                    CustomerEmail = c.Email;
                }
            }

            VecieDati.Text = "Pasūtījuma numurs: " + number.ToString()  +
                             "\nStāvoklis: " + dt.Rows[pasutijumaIndex]["State"].ToString() +
                             "\nDatums: " + date1.ToString() +
                             "\nPasūtītājs: " + CustomerName.ToString() + "\n" + CustomerSurname.ToString() + "\n" +CustomerEmail.ToString();
        }
        private void ShowInputFields(object sender, RoutedEventArgs e)
        {
            if (cboUpdateChoise.SelectedIndex == 0) { PasutNumTxt.Visibility = Visibility.Visible;  MainisanasTeksts.Content = "Jauns pasūtījuma numurs: "; }
            else if (cboUpdateChoise.SelectedIndex == 1) { cboStavoklis.Visibility = Visibility.Visible;  MainisanasTeksts.Content = "Jauns stāvoklis:"; }
            else if (cboUpdateChoise.SelectedIndex == 2) { PasutijumaDatums.Visibility = Visibility.Visible;  MainisanasTeksts.Content = "Jauns pasūtījuma datums:"; }
            else if (cboUpdateChoise.SelectedIndex == 3) { cboPasutitajs.Visibility = Visibility.Visible;  MainisanasTeksts.Content = "Jauns pasūtītajs: "; }
            Mainit.Visibility = Visibility.Hidden;
            MainisanasTeksts.Visibility = Visibility.Visible;
            IzvelejatiesKoMainit.Visibility = Visibility.Hidden;
            cboUpdateChoise.Visibility = Visibility.Hidden;
            RedigetPasutijumu.Visibility = Visibility.Visible;
            AtpakalBtn.Visibility = Visibility.Visible;
            
        }
        private void UpdateOrderClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Vai rediģēt pasūtījumu?\r\n", "Pasūtījuma rediģēšana", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            bool allCorrect = true;
            if (result == MessageBoxResult.OK)
            {
                
                //meklējam pasūtījuma indexu 
                using (var db = new TestDBContext())
                {
                    var PasutNumber = from pasut in db.Orders
                                      where String.Equals((String)dt.Rows[pasutijumaIndex]["Number"], (String)pasut.Number)
                                      select pasut.Id;
                    foreach (var c in PasutNumber) { pasutijumaID = c; }
                }

                //aizpildām lietotāja tabulu
                DataRow drOrder = dt.Rows[pasutijumaIndex];
                drOrder.BeginEdit();

                //ja mēs rediģējam pasūtījuma numuru
                if (PasutNumTxt.Visibility == Visibility.Visible) {
                    //pārbaudam vai datu bāzē ir šis pasūtījuma numurs , jau ir -> kļūda
                    using (var db = new TestDBContext())
                    {
                        var PasutNumber = from pasut in db.Orders
                                          where pasut.Number == PasutNumTxt.Text
                                          select pasut.Id;
                        if (PasutNumber.Any()) { allCorrect = false; }
                        else { foreach (var c in PasutNumber) { pasutijumaID = c; } }
                    }
                    if (allCorrect) {number = PasutNumTxt.Text; drOrder["Number"] = number; }
                    else { drOrder.CancelEdit();  MessageBox.Show("Šis pasūtījuma numurds datu bāzē jau eksistē!", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); }
                }

                //ja mēs rediģējam stāvkoli
                if (cboStavoklis.Visibility == Visibility.Visible)
                {
                    if (cboStavoklis.SelectedItem == null) 
                    {  //ja nav ievadīts 
                        allCorrect = false; drOrder.CancelEdit(); 
                        MessageBox.Show("Lūdzu ievadiet jauno stāvokli!", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); 
                    }
                    else 
                    { 
                        stateIndex  = cboStavoklis.SelectedIndex;  
                        drOrder["State"] = cboStavoklis.SelectedItem; 
                    }
                }

                //ja mēs rediģējam datumu
                if (PasutijumaDatums.Visibility == Visibility.Visible)
                {
                    //nav norādīts datums -> kļūda
                    if (!PasutijumaDatums.SelectedDate.HasValue) { 
                        drOrder.CancelEdit(); 
                        allCorrect = false; 
                        MessageBox.Show("Lūdzu ievadiet jauno datumu!", "Pasūtījuma rediģēšana", MessageBoxButton.OK, MessageBoxImage.Error); 
                    }
                    else
                    {
                        //pārbaudam un pārdefinējam tipu (stingd => DateTime)
                        DateTime.TryParse(PasutijumaDatums.Text, out date1);
                        drOrder["OrderDate"] = date1;
                    }
                }

                //ja mēs rediģējam pasūtītāju
                if (cboPasutitajs.Visibility == Visibility.Visible)
                {   
                    //mēklējam pasūtītāju tabulā un attiecīgus vārdu, uzvārdu, email
                    using (var db = new TestDBContext())
                     {
                            var qryName = from cust in db.Customers
                                          where String.Equals(cust.Name + " " + cust.Surname + " " + cust.Email, cboPasutitajs.SelectedItem.ToString())
                                          select new { cust.Id, cust.Name, cust.Surname, cust.Email };
                            foreach (var c in qryName)
                            {
                                CustomerID = c.Id;
                                drOrder["Name"] = c.Name;
                                drOrder["Email"] = c.Email;
                                drOrder["Surname"] = c.Surname;
                            }
                      }
                }

                if (allCorrect)
                {
                    drOrder.EndEdit();
                    SqlConnection cnNorthwind = new SqlConnection();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    if (ds.HasChanges(DataRowState.Modified))
                    {
                        try
                        {
                            //connection
                            cnNorthwind.ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                            cnNorthwind.Open();
                            //atjauninām pasūtījumu
                            adapter.UpdateCommand = new SqlCommand("UPDATE Orders SET Number=@number, State= @state, OrderDate=@date, CustomerID=@CustomerID WHERE ID=@PasutijumaID", cnNorthwind);
                            adapter.UpdateCommand.Parameters.Add("@PasutijumaID", SqlDbType.Int).Value = pasutijumaID;
                            adapter.UpdateCommand.Parameters.Add("@number", SqlDbType.NVarChar).Value = number;
                            adapter.UpdateCommand.Parameters.Add("@state", SqlDbType.Int).Value = stateIndex;
                            adapter.UpdateCommand.Parameters.Add("@date", SqlDbType.DateTime2).Value = date1;
                            adapter.UpdateCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
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
                                MessageBox.Show("Veiksmigi rediģēts pasūtījums!\r\n", "Pasūtījuma rediģēšana", MessageBoxButton.OK);
                                this.Close(); //aizvēram šo logu 
                            }
                        }
                    }
                }
            }
        }
        public void BackClick(object sender, RoutedEventArgs e)
        {
            Mainit.Visibility = Visibility.Visible;
            IzvelejatiesKoMainit.Visibility = Visibility.Visible;
            cboUpdateChoise.Visibility = Visibility.Visible;
            MainisanasTeksts.Visibility = Visibility.Hidden;
            RedigetPasutijumu.Visibility = Visibility.Hidden;
            AtpakalBtn.Visibility = Visibility.Hidden;
            if (PasutNumTxt.Visibility == Visibility.Visible) { PasutNumTxt.Visibility = Visibility.Hidden; }
            if (cboStavoklis.Visibility == Visibility.Visible) { cboStavoklis.Visibility = Visibility.Hidden; }
            if (PasutijumaDatums.Visibility == Visibility.Visible) { PasutijumaDatums.Visibility = Visibility.Hidden; }
            if (cboPasutitajs.Visibility == Visibility.Visible) { cboPasutitajs.Visibility = Visibility.Hidden; }
        }
    }
}
