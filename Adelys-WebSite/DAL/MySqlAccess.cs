using Adelys_WebSite.DAL.Interface;
using MySql.Data.MySqlClient;

namespace Adelys_WebSite.DAL
{
    public class MySqlAccess : IMySqlAccess
    {
        private MySqlConnection connection;
        //private string server;
        //private string database;
        //private string uid;
        //private string password;

        //Constructor
        public MySqlAccess()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            //server = "localhost";
            //database = "connectcsharptomysql";
            //uid = "username";
            //password = "password";
            string connectionString;
            connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTIONSTRING"); //"SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }
        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
