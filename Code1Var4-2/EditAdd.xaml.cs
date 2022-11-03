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
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace Code1Var4_2
{
    /// <summary>
    /// Логика взаимодействия для EditAdd.xaml
    /// </summary>
    public partial class EditAdd : Window
    {
        public string ConnectionString = @"Data Source=DESKTOP-NG29CEL;Initial Catalog=Code11Var4;Integrated Security=True";
        int TaskID { get; set; }
        LongFormTask task = new LongFormTask();
        ObservableCollection<string> Statuses = new ObservableCollection<string>(){ "запланирована", 
                                                                                    "исполняется", 
                                                                                    "выполнена", 
                                                                                    "отменена" };
        ObservableCollection<string> WorkTypes = new ObservableCollection<string>(){
            "анализ и проектирование",
            "установка оборудования",
            "техническое обслуживание и сопровождение", };
        ObservableCollection<string> Executors = new ObservableCollection<string>();

        public void GetExecutors()
        {
            var sqlConn = new SqlConnection(ConnectionString);
            sqlConn.Open();
            var sql = "select CONCAT(Users.MiddleName, ' ', Users.FirstName, ' ', Users.LastName, ' (ID=', Users.ID, ')') from Users";
            var sqlCommand = new SqlCommand(sql, sqlConn);
            var rd = sqlCommand.ExecuteReader();
            while (rd.Read())
            {
                Executors.Add(rd.GetString(0));
            }
            executorCB.ItemsSource = Executors;
        }
        public void FillExistingTask()
        {
            var sqlConn = new SqlConnection(ConnectionString);
            sqlConn.Open();
            var sql = $"select * from Task where Task.ID = {TaskID}";
            var sqlCommand = new SqlCommand(sql, sqlConn);
            var rd = sqlCommand.ExecuteReader();
            if (rd.Read())
            {
                task = new LongFormTask
                {
                    ID = rd.GetInt32(0),
                    ExecutorID = rd.GetInt32(1),
                    Title = rd.GetString(2),
                    CreateDateTime = rd.GetDateTime(4),
                    Deadline = rd.GetDateTime(5),
                    Difficulty = rd.GetFloat(6),
                    Time = rd.GetInt32(7),
                    Status = rd.GetString(8),
                    WorkType = rd.GetString(9),
                };
                if (rd.IsDBNull(3))
                {
                    task.Description = "";
                }
                else {
                    task.Description = rd.GetString(3);
                }
                if (rd.IsDBNull(10))
                {
                    task.CompletedDateTime = null;
                }
                else {
                    task.CompletedDateTime = rd.GetDateTime(10);
                }
            }
            sqlConn.Close();
        }
        public void FillFields()
        {
            var sqlConn = new SqlConnection(ConnectionString);
            sqlConn.Open();
            var sql = $"select CONCAT(Users.MiddleName, ' ', Users.FirstName, ' ', Users.LastName, ' (ID=', Users.ID, ')' from Users where Users.ID = {task.ExecutorID}";
            var sqlCommand = new SqlCommand(sql, sqlConn);
            var rd = sqlCommand.ExecuteReader();
            if (rd.Read())
            {
                executorCB.SelectedItem = rd.GetString(0);
            }
            sqlConn.Close();

            titileTB.Text = task.Title;
            descriptionTB.Text = task.Description;
            deadlineDP.DataContext = task.Deadline;
            if (task.CompletedDateTime != null)
            {
                completedCB.IsChecked = true;
                completedDP.DataContext = task.CompletedDateTime;
            }
            else
            {
                completedCB.IsChecked = false;
                completedDP.DataContext = null;
            }
            difficultyTB.Text = task.Difficulty.ToString();
            timeTB.Text = task.Time.ToString();
            statusCB.SelectedItem = task.Status;
            workTypeCB.SelectedItem = task.WorkType;
        }
        public EditAdd(int taskID)
        {
            InitializeComponent();
            TaskID = taskID;
            GetExecutors();
            statusCB.ItemsSource = Statuses;
            workTypeCB.ItemsSource = WorkTypes;
            if (TaskID != 0)
            {
                FillExistingTask();
            }
        }
    }
}
