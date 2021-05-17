using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        DataBase dataBase;
        string CurrentStudent;
        int ClientID;
        MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
        public StudentForm(int id, string StudentFIO, DataBase db)
        {
            InitializeComponent();
            dataBase = db;
            CurrentStudent = StudentFIO;
            ClientID = id;
            parseOrders();
        }

        private void parseOrders()
        {
            DataTable tables = new DataTable();
            try
            {
                MySqlCommand tablesParse = new MySqlCommand("select Name from computercourses.computer_course as J cross join (select ID_Course from computercourses.orders where ID_Client="+ClientID+") as T on  J.ID_Course=T.ID_Course", dataBase.getConnection());
                dataAdapter.SelectCommand = tablesParse;
                dataAdapter.Fill(tables);
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

            CoursesView.ItemsSource = new DataView(tables);
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void tb_price_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
