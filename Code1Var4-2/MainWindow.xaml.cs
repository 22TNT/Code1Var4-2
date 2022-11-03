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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.Security.Cryptography;
using Code1Var4_2;
using System.Diagnostics;

namespace Code1Var4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string HashPassword(string login, string password, string salt)
        {
            using (var provider = MD5.Create())
            {
                StringBuilder builder = new StringBuilder();

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(login+password+salt)))
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();

            }
        }
        public string ConnectionString = @"Data Source=DESKTOP-NG29CEL;Initial Catalog=Code11Var4;Integrated Security=True";
        public string Login { get; set; }
        public string Password { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this.DataContext;
        }

        private bool CheckIfManager(int id)
        {
            var sqlConn = new SqlConnection { ConnectionString = ConnectionString };
            sqlConn.Open();
            var sql = $"select * from Manager where ID={id}";
            var sqlCommand = new SqlCommand(sql, sqlConn);
            var rd = sqlCommand.ExecuteReader();
            if (rd.HasRows)
            {
                return true;
            }
            return false;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = loginTB.Text;
            var password = passwordTB.Text;
            Trace.WriteLine(Login + Password);
            const string salt = "CorrectHorseBatteryStaple";

            var sqlConn = new SqlConnection { ConnectionString = ConnectionString };
            sqlConn.Open();
            Trace.WriteLine(HashPassword(login, password, salt));
            var sql = $"select Users.ID from Users where Users.Login='{login}' and Users.Password='{HashPassword(login, password, salt)}' and Users.IsDeleted=0";
            var sqlCommand = new SqlCommand(sql, sqlConn);
            var rd = sqlCommand.ExecuteReader();
            if (rd.HasRows)
            {
                rd.Read();
                if (CheckIfManager(Int32.Parse(rd[0].ToString())))
                {
                    var window = new Manager(Int32.Parse(rd[0].ToString()));
                    window.Show();
                }
                else
                {
                    var window = new Users(Int32.Parse(rd[0].ToString()));
                    window.Show();
                }
            }
            else
            {
                MessageBox.Show("Invalid credentials");
            }
        }
    }
}
