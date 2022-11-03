using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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

namespace Code1Var4_2
{
    /// <summary>
    /// Логика взаимодействия для Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        public int UserID { get; set; }
        public string ConnectionString = @"Data Source=DESKTOP-NG29CEL;Initial Catalog=Code11Var4;Integrated Security=True";
        public ObservableCollection<string> ExecutorNames = new ObservableCollection<string>();
        public ObservableCollection<string> TaskStatus = new ObservableCollection<string>();
        public ObservableCollection<Task> Tasks { get; set; }

        public void SetInitialFilters()
        {
            TaskStatus.Add("Все");
            statusComboBox.ItemsSource = TaskStatus;
            statusComboBox.SelectedIndex = 0;
        }

        public void GetTaskStatuses()
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var sql = $"select distinct Task.Status from Task";
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            var rd = sqlCommand.ExecuteReader();
            while (rd.Read())
            {
                TaskStatus.Add(rd.GetString(0));
            }
            sqlConnection.Close();
        }

        public void RenderTasks()
        {
            string statusFilter = "";
            if (statusComboBox.SelectedIndex != -1 && statusComboBox.SelectedItem.ToString() != "Все")
            {
                statusFilter = $"and Task.Status = '{statusComboBox.SelectedItem.ToString()}' ";
            }

            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var sql = $"select Task.ID as [ID], Task.Title as [Заголовок], Task.Status as [Статус], " +
                      $"(select top 1 CONCAT(Users.MiddleName, ' ', Users.FirstName, ' ', Users.LastName) from Users where Users.ID = Executor.ManagerID) as [ФИО менеджера], " +
                      $"CONCAT(Users.MiddleName, ' ', Users.FirstName, ' ', Users.LastName) as [ФИО исполнителя] " +
                      "from Task, Executor, Manager, Users " +
                      $"where Users.ID = {UserID} and Executor.ManagerID = Manager.ID and Executor.ID = Users.ID and Task.ExecutorID = Executor.ID {statusFilter} " +
                      $"order by Task.CreateDateTime desc";
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            var rd = sqlCommand.ExecuteReader();
            Tasks = new ObservableCollection<Task>();
            while (rd.Read())
            {
                Tasks.Add(new Task
                {
                    ID = rd.GetInt32(0),
                    Title = rd.GetString(1),
                    Status = rd.GetString(2),
                    Name = rd.GetString(3),
                    ManagerName = rd.GetString(4),
                }) ;
            }
            tasksDG.ItemsSource = Tasks;
            sqlConnection.Close();
        }

        public Users(int id)
        {
            UserID = id;
            InitializeComponent();
            SetInitialFilters();
            GetTaskStatuses();
            RenderTasks();
        }

        private void statusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RenderTasks();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = (Task)tasksDG.SelectedItem;
                var sqlConn = new SqlConnection(ConnectionString);
                sqlConn.Open();
                var sql = $"UPDATE Task SET IsDeleted=1 WHERE Task.ID = {selectedTask.ID}";
                var sqlCommand = new SqlCommand(sql, sqlConn);
                sqlCommand.ExecuteNonQuery();
                sqlConn.Close();
                RenderTasks();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
