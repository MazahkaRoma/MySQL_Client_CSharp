using System;
using System.Collections.Generic;
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
        public StudentForm(string StudentFIO, DataBase db)
        {
            InitializeComponent();
            dataBase = db;
            CurrentStudent = StudentFIO;
        }
    }
}
