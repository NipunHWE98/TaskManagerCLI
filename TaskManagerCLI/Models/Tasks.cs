using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TaskManagerCLI.Models
{
    public class Tasks
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskCategory Category { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool IsAdminOnly { get; set; } = false;

        // Check if the task is overdue
        public bool IsOverdue => !IsComplete && DueDate < DateTime.Now;

        public Tasks(string title, string description, DateTime dueDate, TaskPriority priority, TaskCategory category, bool isAdminOnly = false)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Category = category;
            IsAdminOnly = isAdminOnly;
        }

        public Tasks()
        {
        }

        //public static implicit operator Tasks(Tasks v) => throw new NotImplementedException();
    }
}

