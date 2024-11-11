using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TaskManagerCLI.Models;

namespace TaskManagerCLI.Services
{
    public class TaskService
    {
        private List<Tasks> tasks = new();
        private readonly string filePath = "tasks.json";
        private readonly bool isAdmin;

        public TaskService(bool isAdmin)
        {
            this.isAdmin = isAdmin;
            LoadTasks();
        }

        public void AddTask(string title, string description, DateTime dueDate, string priority, string category, bool isAdminOnly = false)
        {
            if (isAdminOnly && !isAdmin)
            {
                Console.WriteLine("Only admins can add admin-only tasks.");
                return;
            }
            var task = new Tasks()
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = ConvertStringToPriority(priority),
                Category = ConvertStringToTaskCategory(category),
                IsAdminOnly = isAdminOnly
            };
            tasks.Add(task);
            SaveTasks();
            Console.WriteLine("Task added successfully!");
        }

        public IEnumerable<Tasks> ViewTasks()
        {
            return isAdmin ? tasks : tasks.FindAll(t => !t.IsAdminOnly);
        }

        public void FilterTasksByCategory(string category)
        {
            TaskCategory taskCategory = ConvertStringToTaskCategory(category);

            
            var filteredTasks = isAdmin
                ? tasks.FindAll(t => t.Category == taskCategory)
                : tasks.FindAll(t => t.Category == taskCategory && !t.IsAdminOnly);

            if (filteredTasks.Count > 0)
            {
                Console.WriteLine($"Tasks in the '{taskCategory}' category:");
                foreach (var task in filteredTasks)
                {
                    Console.WriteLine($"ID: {task.ID} | Title: {task.Title} | Due: {task.DueDate.ToShortDateString()} | Complete: {task.IsComplete} | AdminOnly: {task.IsAdminOnly}");
                }
            }
            else
            {
                Console.WriteLine($"No tasks found in the '{taskCategory}' category.");
            }
        }


        public void FilterTasksByPriority(string priority)
        {
            TaskPriority taskPriority = ConvertStringToPriority(priority);


            var filteredTasks = isAdmin
                ? tasks.FindAll(t => t.Priority == taskPriority)
                : tasks.FindAll(t => t.Priority == taskPriority && !t.IsAdminOnly);

            if (filteredTasks.Count > 0)
            {
                Console.WriteLine($"Tasks in the '{taskPriority}' priority level:");
                foreach (var task in filteredTasks)
                {
                    Console.WriteLine($"ID: {task.ID} | Title: {task.Title} | Due: {task.DueDate.ToShortDateString()} | Complete: {task.IsComplete} | AdminOnly: {task.IsAdminOnly}");
                }
            }
            else
            {
                Console.WriteLine($"No tasks found in the '{taskPriority}' category.");
            }
        }

        public void ListCategories()
        {
            Console.WriteLine("Available Categories:");
            foreach (var category in Enum.GetValues(typeof(TaskCategory)))
            {
                Console.WriteLine($"- {category}");
            }
        }

        public void ListPriorities()
        {
            Console.WriteLine("Available Priorities:");
            foreach (var priority in Enum.GetValues(typeof(TaskPriority)))
            {
                Console.WriteLine($"- {priority}");
            }
        }

        public void MarkComplete(Guid id)
        {
            var task = tasks.Find(t => t.ID == id);
            if (task != null)
            {
                task.IsComplete = true;
                SaveTasks();
                Console.WriteLine("Task marked as complete.");
            }
        }

        public void RemoveTask(Guid id)
        {
            tasks.RemoveAll(t => t.ID == id);
            SaveTasks();
        }

        private void SaveTasks()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(tasks, Formatting.Indented));
        }

        private void LoadTasks()
        {
            if (File.Exists(filePath))
                tasks = JsonConvert.DeserializeObject<List<Tasks>>(File.ReadAllText(filePath));
        }

        private TaskCategory ConvertStringToTaskCategory(string category)
        {
            
            string categoryCode = category.ToUpper().Substring(0, Math.Min(3, category.Length));

           
            foreach (TaskCategory taskCategory in Enum.GetValues(typeof(TaskCategory)))
            {
                
                string taskCategoryCode = taskCategory.ToString().ToUpper().Substring(0, 3);

               
                if (taskCategoryCode == categoryCode)
                {
                    return taskCategory;
                }
            }

            
            return TaskCategory.OTHER;
        }

        private TaskPriority ConvertStringToPriority(string priority)
        {
            if (Enum.TryParse(priority, true, out TaskPriority result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"Invalid priority: {priority}. Valid values are P1, P2, P3, P4, P5.");
            }

        }
        public void SortTasksByDueDate(string order)
        {
            bool ascending=true;
            bool valid=false;   
            if (order == "asc")
            {
                ascending = true;
                valid = true;   
            }
            if (order == "dec")
            {
                ascending = false;
                valid = true;
            }
            else {
                Console.WriteLine("Incorrect usage sort asc/dec");
            }
            if (valid)
            {
                if (ascending)
                {
                    tasks.Sort((task1, task2) => task1.DueDate.CompareTo(task2.DueDate));
                    Console.WriteLine("Tasks sorted in ascending order by due date.");
                }
                else
                {
                    tasks.Sort((task1, task2) => task2.DueDate.CompareTo(task1.DueDate));
                    Console.WriteLine("Tasks sorted in descending order by due date.");
                }

                Console.WriteLine("Sorted Tasks:");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"ID: {task.ID} | Title: {task.Title} | Due: {task.DueDate.ToShortDateString()} | Complete: {task.IsComplete} | AdminOnly: {task.IsAdminOnly}");
                }
            }
        }

        public void EditCategory(Guid id, string category)
        {
            var task = tasks.Find(t => t.ID == id);
            if (task != null)
            {
                task.Category = ConvertStringToTaskCategory(category);
                SaveTasks();  
                Console.WriteLine($"Task ID {id} category updated to {category}.");
            }
            else
            {
                Console.WriteLine($"Task with ID {id} not found.");
            }
        }

        public void EditPriority(Guid id, string priority)
        {
            var task = tasks.Find(t => t.ID == id);
            if (task != null)
            {
                task.Priority = ConvertStringToPriority(priority);
                SaveTasks();
                Console.WriteLine($"Task ID {id} category updated to {priority}.");
            }
            else
            {
                Console.WriteLine($"Task with ID {id} not found.");
            }
        }

    }
}
