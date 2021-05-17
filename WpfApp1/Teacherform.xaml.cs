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
    /// Логика взаимодействия для Teacherform.xaml
    /// </summary>
    public partial class Teacherform : Window
    {
        DataBase dataBase;
        MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
        string CurrentTeacher;
        int teacherID;
        string CourseName;
        int CourseID;
        public Teacherform(int TeacherID, string TeacherFIO, DataBase db)
        {
            InitializeComponent();
            teacherID = TeacherID;
            MessageBox.Show(teacherID.ToString());
            CurrentTeacher = TeacherFIO;
            dataBase = db;
            parseDataBaseTables();
        }

        private void parseDataBaseTables()
        {
            DataTable tables = new DataTable();
            try
            {
                MySqlCommand tablesParse = new MySqlCommand("SELECT NAME FROM computercourses.computer_course WHERE ID_Teacher ="+teacherID.ToString()+";" , dataBase.getConnection());
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

        private void StackPanel_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            CourseName = ((TextBlock)sender).Text;
            DataTable courseInfo = new DataTable();
            MySqlCommand infoParse = new MySqlCommand("SELECT * FROM computercourses.computer_course WHERE Name =" + '"'+CourseName + '"'+ ";", dataBase.getConnection());
            dataAdapter.SelectCommand = infoParse;
            dataAdapter.Fill(courseInfo);

            CourseID = (int)courseInfo.Rows[0].ItemArray[0];
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
            if(!((TextBox)sender).Text.Contains("$"))
                ((TextBox)sender).Text += "$";
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            string newName = tb_courseName.Text;
            string newPrice = tb_price.Text;
            int newDayOfreturn = Int32.Parse(tb_daysOfReturn.Text);
            string newCategory = tb_category.Text;
            int category_id = 0;
            try
            {
                MySqlCommand command = new MySqlCommand("SELECT ID_Category FROM computercourses.category WHERE Type=" + '"' + newCategory + '"', dataBase.getConnection());
                DataTable tmp = new DataTable();
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(tmp);

                category_id = (int)tmp.Rows[0].ItemArray[0];
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

            try
            {
                MySqlCommand insertCommand = new MySqlCommand("UPDATE computercourses.computer_course SET NAME=" + '"' + newName + '"' + " WHERE ID_Course=" + CourseID + ";", dataBase.getConnection());
                insertCommand.ExecuteNonQuery();

                insertCommand = new MySqlCommand("UPDATE computercourses.computer_course SET Price=" + '"' + newPrice + '"' + " WHERE ID_Course=" + CourseID + ";", dataBase.getConnection());
                insertCommand.ExecuteNonQuery();

                insertCommand = new MySqlCommand("UPDATE computercourses.computer_course SET Date_of_Return=" + newDayOfreturn + " WHERE ID_Course=" + CourseID + ";", dataBase.getConnection());
                insertCommand.ExecuteNonQuery();

                insertCommand = new MySqlCommand("UPDATE computercourses.computer_course SET ID_Category=" + category_id + " WHERE ID_Course=" + CourseID + ";", dataBase.getConnection());
                insertCommand.ExecuteNonQuery();
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }
            parseDataBaseTables();
        }

        private void btn_addNewCourse_Click(object sender, RoutedEventArgs e)
        {
            string Name = tb_newCourseName.Text;
            string Price = tb_newPrice.Text;
            int DayOfreturn = Int32.Parse(tb_newDaysOfReturn.Text);
            string Category = tb_newCategory.Text;
            int category_id;
            try
            {
                MySqlCommand command = new MySqlCommand("SELECT ID_Category FROM computercourses.category WHERE Type=" + '"' + Category + '"', dataBase.getConnection());
                DataTable tmp = new DataTable();
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(tmp);

                category_id = (int)tmp.Rows[0].ItemArray[0];
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }

            try
            {
                MySqlCommand insertcommand = new MySqlCommand("INSERT computercourses.computer_course(NAME, Date_Of_Return, Price, ID_Teacher, ID_Category) VALUES('" + Name + "'," + DayOfreturn + ",'" + Price + "'," + teacherID + "," + category_id + ");", dataBase.getConnection());

                insertcommand.ExecuteNonQuery();
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message, error.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return;
            }
            parseDataBaseTables();
        }
    }

}
