using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {

        private MySqlDataAdapter dataAdapter;
        private DataBase dataBase;
        private string CurrentTable;
        private DataTable newTable = new DataTable();

        private ObservableCollection<Column> LstColumns { get ; set ; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            dataBase = new DataBase();
            dataAdapter = new MySqlDataAdapter();
            tabControl.Visibility = Visibility.Collapsed;
            DataBaseView.Visibility = Visibility.Collapsed;
            AddTable.Visibility = Visibility.Hidden;
            btn_Appy.IsEnabled = false;
            LstColumns = new ObservableCollection<Column>();
            NewTableData.ItemsSource = LstColumns;

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
            DataTable loginDT = new DataTable();

            try
            {
                dataBase.LocalConnect("Admin", "123456789");
                MySqlCommand tableDataParse = new MySqlCommand("SELECT ID_Teacher,FIO,Pasport FROM computercourses.teacher WHERE FIO="+ '"' + Login + '"' + " AND Pasport="+ '"' + Password+ '"'+";", dataBase.getConnection());
                dataAdapter.SelectCommand = tableDataParse;
                
                if(Login=="admin" && Password=="admin")
                {

                    LogInGrid.Visibility = Visibility.Collapsed;
                    tabControl.Visibility = Visibility.Visible;
                    DataBaseView.Visibility = Visibility.Visible;

                    ParseDataBaseData();
                    return;
                }

                if (dataAdapter.Fill(loginDT) != 0)
                {
                    Teacherform tc = new Teacherform((int)loginDT.Rows[0].ItemArray[0], Login, dataBase);
                    this.Hide();
                    tc.Show();
                    this.Close();
                    return;
                }
                else
                {
                    tableDataParse = new MySqlCommand("SELECT * FROM computercourses.client WHERE FIO=" + '"' + Login + '"' + " AND Phone=" + '"' + Password + '"' + ";", dataBase.getConnection());
                    dataAdapter.SelectCommand = tableDataParse;
                    DataTable dt = new DataTable();
                    if (dataAdapter.Fill(dt) != 0)
                    {
                        this.Hide();
                        new StudentForm((int)dt.Rows[0].ItemArray[0], Login, dataBase).Show();
                        this.Close();
                        return;

                    } 
                }


                MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }



        }



        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataTable tableData = new DataTable();

            TextBlock tableName = (TextBlock)(sender);

            CurrentTable = tableName.Text;

            TableItem.Header = CurrentTable;
            parseTable(CurrentTable, tableData);

            TableData.DataContext = tableData.DefaultView;
        }


        private void parseTable(string tableName, DataTable table_to_Fill)
        {
            try
            {
                MySqlCommand tableDataParse = new MySqlCommand("SELECT * FROM `" + tableName + "`;", dataBase.getConnection());
                dataAdapter.SelectCommand = tableDataParse;
            }
            catch(MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

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
            try
            {
                MySqlCommand tablesParse = new MySqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", dataBase.getConnection());
                dataAdapter.SelectCommand = tablesParse;
                dataAdapter.Fill(tables);
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

            TableList.ItemsSource = new DataView(tables);


        }

        private void parseDataBaseViews()
        {
            DataTable views = new DataTable();
            try
            {
                MySqlCommand tablesParse = new MySqlCommand("SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_TYPE LIKE 'VIEW'; ", dataBase.getConnection());
                dataAdapter.SelectCommand = tablesParse;

                dataAdapter.Fill(views);
            }
            catch(MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

            //ViewsList.ItemsSource = new DataView(views);
        }

        private void parseDataBaseTriggers()
        {
            DataTable views = new DataTable();
            try
            {
                MySqlCommand tablesParse = new MySqlCommand("SELECT trigger_name FROM information_schema.triggers;", dataBase.getConnection());
                dataAdapter.SelectCommand = tablesParse;
            }
            catch(MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }
            dataAdapter.Fill(views);
           

            //TriggersList.ItemsSource = new DataView(views);
        }

        private void TriggerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btn_Appy_Click(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = ((DataView)(TableData.ItemsSource)).ToTable();

            DataTable originalTable = new DataTable();
            parseTable(CurrentTable, originalTable);

            if (originalTable.Rows.Count < dataTable.Rows.Count)
            {
                DataTable ds = new DataTable();
                ds = GetDataTableLayout(CurrentTable);
                for (int i = originalTable.Rows.Count; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = ds.NewRow();
                    row.ItemArray = dataTable.Rows[i].ItemArray;
                    ds.Rows.Add(row);
                }

                try
                {
                    BulkInsertMySQL(ds, CurrentTable);
                }
                catch (MySqlException error)
                {
                    MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                    return;
                }
            }
        }


        public DataTable GetDataTableLayout(string tableName)
        {
            DataTable table = new DataTable();
            try
            {
                string query = $"SELECT * FROM " + tableName + " limit 0";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataBase.getConnection()))
                {
                    adapter.Fill(table);
                };
            }
            catch(MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return null;
            }

            return table;
        }

        public void BulkInsertMySQL(DataTable table, string tableName)
        {
            try
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
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

        }

        private void TableData_RowEditEnding(object sender, SelectionChangedEventArgs e)
        {
            btn_Appy.IsEnabled = true;
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            AddTable.Visibility = Visibility.Visible;
            tabControl.SelectedItem = AddTable;
        }

        private void NewTableData_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            LstColumns.Add(new Column());
            NewTableData.ItemsSource = LstColumns;
        }

        private void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            StackPanel sp = null;
            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as StackPanel;
            }

            string cmdLine = "DROP TABLE " + ((TextBlock)(sp.Children[2])).Text + ";";

            try
            {
                MySqlCommand cmd = new MySqlCommand(cmdLine, dataBase.getConnection());
                cmd.ExecuteNonQuery();
            }
            catch(MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }

            ParseDataBaseData();
        }

        private void btn_addTable_Click(object sender, RoutedEventArgs e)
        {
            string cmdString = "CREATE TABLE `" + newTableName.Text + "`(";

            foreach (Column column in LstColumns)
            {
                string PK = ",PRIMARY KEY(`" + column.Column_Name + "`)"; ;
                string UQ = ",UNIQUE INDEX `" + column.Column_Name + "_UNIQUE` (`" + column.Column_Name + "` ASC) VISIBLE ";

                cmdString += column.Column_Name;

                cmdString +=" "+ column.type + " NOT NULL";

                if (column.Unique == true)
                {
                    cmdString += UQ;
                }

                if (column.Primary_Key == true)
                {
                    cmdString += PK;
                }
                if(LstColumns.IndexOf(column)!=LstColumns.Count-1) { cmdString += ","; }

            }

            cmdString += ");";

            MessageBox.Show(cmdString);

            try
            {
                MySqlCommand createTableCommand = new MySqlCommand(cmdString, dataBase.getConnection());
                createTableCommand.ExecuteNonQuery();
                ParseDataBaseData();
            }
            catch(MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }

        private void TableData_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {
            btn_Appy.IsEnabled = true;
        }
    }
}
