using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code1Var4_2
{
    public class LongFormTask
    {
        public int ID { get; set; }
        public int ExecutorID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime Deadline { get; set; }
        public float Difficulty { get; set; }
        public int Time { get; set; }
        public string Status { get; set; }
        public string WorkType { get; set; }

        public DateTime? CompletedDateTime { get; set; }
        public bool IsDeleted { get; set; }

        public LongFormTask() {
            ID = 0;
            ExecutorID = 0;
            Title = "";
            Description = "";
            CreateDateTime = DateTime.Now;
            Deadline = DateTime.Now;
            Difficulty = 0;
            Time = 0;
            Status = "запланирована";
            CompletedDateTime = null;
            WorkType = "анализ и проектирование";
            IsDeleted = false;
        }
    }
}
