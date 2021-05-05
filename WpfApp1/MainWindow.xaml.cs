using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace WpfApp1
{
    public partial class MainWindow : Window
    {

        private MySqlDataAdapter dataAdapter;
        private DataBase dataBase;
        private string CurrentTable;
        private DataTable Table;
        bool rowEdited = false;

        public MainWindow()
        {
            InitializeComponent();
            dataBase = new DataBase();
            dataAdapter = new MySqlDataAdapter();
            Table = new DataTable();
            tabControl.Visibility = Visibility.Collapsed;
            DataBaseView.Visibility = Visibility.Collapsed;
        }

        private void btn_LogIn_Click(object sender, RoutedEventArgs e)
        {
            login();

            
        }
        
        private void txt_LogIn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) txt_password.Focus();
        }

        private void txt_password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) login();
        }
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void login()
        {
            string Login = txt_LogIn.Text;
            string Password = txt_password.Password;

            try
            {
                dataBase.LocalConnect(Login, Password);
            }
            catch (MySqlException ooop)
            {
                System.Windows.MessageBox.Show(ooop.Message);
                return;
            }

            

            LogInGrid.Visibility = Visibility.Collapsed;
            tabControl.Visibility = Visibility.Visible;
            DataBaseView.Visibility = Visibility.Visible;

            ParseDataBaseData();
        }

        

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        { 
            DataTable tableData = new DataTable();

            TextBlock tableName = (TextBlock)(sender);

            CurrentTable = tableName.Text;

            parseTable(CurrentTable, tableData);

            TableData.DataContext = tableData.DefaultView;
        }


        private void parseTable(string tableName, DataTable table_to_Fill)
        {
            MySqlCommand tableDataParse = new MySqlCommand("SELECT * FROM `" + tableName + "`;", dataBase.getConnection());
            dataAdapter.SelectCommand = tableDataParse;
            dataAdapter.Fill(table_to_Fill);
        }

        private void ParseDataBaseData()
        {
            parseDataBaseTables();
            parseDataBaseViews();
            parseDataBaseTriggers();
        }
            
        private void parseDataBaseTables()
        {
            DataTable tables = new DataTable();

            MySqlCommand tablesParse = new MySqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", dataBase.getConnection());
            dataAdapter.SelectCommand = tablesParse;

            dataAdapter.Fill(tables);
            
            TableList.ItemsSource=new DataView(tables);
           

        }

        private void parseDataBaseViews()
        {
            DataTable views = new DataTable();

            MySqlCommand tablesParse = new MySqlCommand("SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_TYPE LIKE 'VIEW'; ", dataBase.getConnection());
            dataAdapter.SelectCommand = tablesParse;

            dataAdapter.Fill(views);

            ViewsList.ItemsSource = new DataView(views);
        }

        private void parseDataBaseTriggers()
        {
            DataTable views = new DataTable();

            MySqlCommand tablesParse = new MySqlCommand("SELECT trigger_name FROM information_schema.triggers;", dataBase.getConnection());
            dataAdapter.SelectCommand = tablesParse;
           
            dataAdapter.Fill(views);

            TriggersList.ItemsSource = new DataView(views);
        }

        private void TriggerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btn_Appy_Click(object sender, RoutedEventArgs e)
        {

           
          

          
        }


        public DataTable GetDataTableLayout(string tableName)
        {
            DataTable table = new DataTable();               
            string query = $"SELECT * FROM " + tableName + " limit 0";
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataBase.getConnection()))
            {
                adapter.Fill(table);
            };
            

            return table;
        }

        public void BulkInsertMySQL(DataTable table, string tableName) 
        {

            using (MySqlTransaction tran = dataBase.getConnection().BeginTransaction(IsolationLevel.Serializable))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = dataBase.getConnection();
                    cmd.Transaction = tran;
                    cmd.CommandText = $"SELECT * FROM " + tableName + " limit 0";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd)) 

                    {
                        adapter.UpdateBatchSize = 10000;
                        using (MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter))
                        {
                            cb.SetAllValues = true;
                            adapter.Update(table);
                            tran.Commit();
                        }
                    };
                }
            }

        }

        private void TableData_RowEditEnding(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TableData_LayoutUpdated(object sender, EventArgs e)
        {
            
        }

        private void TableData_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                DataTable ds = new DataTable();
                ds = GetDataTableLayout(CurrentTable);


                DataTable originTable = new DataTable();
                parseTable(CurrentTable, originTable);

                DataTable dataTable = ((DataView)(TableData.ItemsSource)).ToTable();

                DataRow row = ds.NewRow();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    row[i] = dataTable.Rows[dataTable.Rows.Count - 1][i];
                }
                ds.Rows.Add(row);


                try
                {
                    BulkInsertMySQL(ds, CurrentTable);
                }
                catch (MySqlException oops)
                {
                    MessageBox.Show(oops.Message);
                }

                rowEdited = false;
            }
        }
    }
}
