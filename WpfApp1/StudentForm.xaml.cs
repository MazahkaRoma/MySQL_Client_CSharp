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
            parsecourses();

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

        private void parsecourses()
        {
            DataTable courses = new DataTable();

            MySqlCommand parse = new MySqlCommand("select Name from computercourses.computer_course;", dataBase.getConnection());
            dataAdapter.SelectCommand = parse;
            dataAdapter.Fill(courses);
            for(int i = 0; i<courses.Rows.Count;i++)
            {
                cmb_Courses.Items.Add(courses.DefaultView[i]["Name"]);
            }

            courses.Clear();

            parse.CommandText = "select Name from computercourses.sell_method";
            dataAdapter.Fill(courses);

            for (int i = 0; i < courses.Rows.Count; i++)
            {
                cmb_payMethod.Items.Add(courses.DefaultView[i]["Name"]);
            }


        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string CourseName = ((TextBlock)sender).Text;
            DataTable courseInfo = new DataTable();
            MySqlCommand infoParse = new MySqlCommand("SELECT * FROM computercourses.computer_course WHERE Name =" + '"' + CourseName + '"' + ";", dataBase.getConnection());
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(courseInfo);

            int teacherID = (int)courseInfo.Rows[0].ItemArray[4];
            infoParse = new MySqlCommand("select FIO from teacher where ID_Teacher=" + teacherID + ";", dataBase.getConnection());
            DataTable table = new DataTable();
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(table);
            tb_teacher.Text = (string)table.Rows[0].ItemArray[0];
            int CourseID = (int)courseInfo.Rows[0].ItemArray[0];
            int categoryID = (int)courseInfo.Rows[0].ItemArray[5];
            infoParse = new MySqlCommand("SELECT Type FROM computercourses.category WHERE ID_Category = " + categoryID + ";", dataBase.getConnection());
            DataSet set = new DataSet();
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(set);
            string type = (string)set.Tables[0].Rows[0].ItemArray[0];
            tb_courseName.Text = CourseName;
            tb_category.Text = type;
            tb_daysOfReturn.Text = ((int)courseInfo.Rows[0].ItemArray[2]).ToString();
            tb_price.Text = (string)courseInfo.Rows[0].ItemArray[3];
        }

        private void tb_price_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cmb_Courses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox tmp = (ComboBox)sender;
            string si = tmp.SelectedItem.ToString();

            DataTable courseInfo = new DataTable();
            MySqlCommand infoParse = new MySqlCommand("SELECT * FROM computercourses.computer_course WHERE Name =" + '"' + si + '"' + ";", dataBase.getConnection());
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(courseInfo);

            int teacherID = (int)courseInfo.Rows[0].ItemArray[4];
            infoParse = new MySqlCommand("select FIO from teacher where ID_Teacher=" + teacherID + ";", dataBase.getConnection());
            DataTable table = new DataTable();
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(table);
            tb_teacher_Copy.Text = (string)table.Rows[0].ItemArray[0];
            int CourseID = (int)courseInfo.Rows[0].ItemArray[0];
            int categoryID = (int)courseInfo.Rows[0].ItemArray[5];
            infoParse = new MySqlCommand("SELECT Type FROM computercourses.category WHERE ID_Category = " + categoryID + ";", dataBase.getConnection());
            DataSet set = new DataSet();
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(set);
            string type = (string)set.Tables[0].Rows[0].ItemArray[0];

            tb_category_Copy.Text = type;
            tb_daysOfReturn_Copy.Text = ((int)courseInfo.Rows[0].ItemArray[2]).ToString();
            tb_price_Copy.Text = (string)courseInfo.Rows[0].ItemArray[3];
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand insertcmd = new MySqlCommand("insert computercourses.orders(ID_Client,ID_Course,ID_Manager,Date,Price,ID_SellMethod) values(" + ClientID + ",(select ID_Course from computercourses.computer_course where Name=" + '"' + cmb_Courses.Text + '"' + "),(select ID_Manager from computercourses.manager ORDER BY RAND() limit 1),CURDATE()," + '"' + tb_price_Copy.Text + '"' + ",(select ID_Method from computercourses.sell_method where Name=" + '"' + cmb_payMethod.Text + '"' + "));", dataBase.getConnection());
                insertcmd.ExecuteNonQuery();
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }
            parseOrders();
        }
    }
}
