using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DataBase
    {
        private MySqlConnection connection;

        ~DataBase()
        {
        }

        public void LocalConnect(string login,string password)
        {
            connection = new MySqlConnection("server=localhost;port=3306;username=" + login + ";password=" + password + ";database=computercourses");
            openConnection();
            
        }

        public void RemoteConnect(string login, string password, string IP)
        {

        }

        public bool isConnected()
        {
          return connection.State == System.Data.ConnectionState.Open;
        }

        private void openConnection()
        {
            if (!isConnected())
            {
                connection.Open();
            }
        }



        private void closeConnection()
        {
            if (isConnected())
            {
                connection.Close();
            }
        }


        public MySqlConnection getConnection()
        {
            return connection;
        }
        
    }
}
