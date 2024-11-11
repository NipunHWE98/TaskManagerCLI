using System;
using System.Globalization;
using TaskManagerCLI.Services;
using TaskManagerCLI.Models;

namespace TaskManagerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: exe [username] [password] [command] [arguments...]");
                return;
            }

            string username = args[0];
            string password = args[1];

           
            User user= User.Authenticate(username, password);
            bool isAdmin=user.IsAdmin;

            if (!isAdmin && password != "userpass")
            {
                Console.WriteLine("Invalid credentials. Access denied.");
                return;
            }

           
            var taskService = new TaskService(isAdmin);

            
            if (args.Length < 3)
            {
                Console.WriteLine("Commands: add, view, complete, remove");
                return;
            }

            string command = args[2].ToLower();
            switch (command)
            {
                case "add":
                    if (args.Length < 8)
                    {
                        Console.WriteLine("Usage: add [title] [description] [dueDate yyyy-MM-dd] [priority] [category] [isAdminOnly]");
                        return;
                    }
                    string title = args[3];
                    string description = args[4];
                    DateTime dueDate = DateTime.ParseExact(args[5], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    string priority = args[6];
                    string category = args[7];
                    bool isAdminOnly = args.Length > 8 && args[8].ToLower() == "true";
                    try
                    {
                        taskService.AddTask(title, description, dueDate, priority, category, isAdminOnly);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                        break;
                case "edit":
                    if (args.Length >= 4 && args[3].ToLower() == "-c")
                    {
                        Guid Id = Guid.Parse(args[4]);
                        taskService.EditCategory(Id, args[5]);
                        break;
                    }
                    if (args.Length >= 4 && args[3].ToLower() == "-p")
                    {
                        Guid Id = Guid.Parse(args[4]);
                        taskService.EditPriority(Id, args[5]);
                        break;

                    }
                    else
                    {
                        Console.WriteLine("Unknown option for list. Use '-c/-p' [ID].");
                        break;
                    }
                   

                case "view":
                    foreach (var task in taskService.ViewTasks())
                    {
                        Console.WriteLine($"ID: {task.ID} | Title: {task.Title} | Due: {task.DueDate.ToShortDateString()} | Complete: {task.IsComplete} | AdminOnly: {task.IsAdminOnly}");
                    }
                    break;

                case "complete":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Usage: complete [taskId]");
                        return;
                    }
                    Guid completeId = Guid.Parse(args[3]);
                    taskService.MarkComplete(completeId);
                    break;

                case "remove":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Usage: remove [taskId]");
                        return;
                    }
                    Guid removeId = Guid.Parse(args[3]);
                    taskService.RemoveTask(removeId);
                    Console.WriteLine("Task removed.");
                    break;

                case "list":
                    if (args.Length >= 4 && args[3].ToLower() == "-c")
                    {
                        Console.WriteLine("Category List:");
                        taskService.ListCategories();
                    }
                    if (args.Length >= 4 && args[3].ToLower() == "-p") {
                        Console.WriteLine("Priority List:");
                        taskService.ListPriorities();
                    
                    }
                    else
                    {
                        Console.WriteLine("Unknown option for list. Use '-c' to list categories '-p' to list catogories.");
                    }
                    break;
                case "filter":
                    if (args[3].ToLower() == "-c") {
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Usage : filter -c [catogory] ");
                            return;
                        }
                        taskService.FilterTasksByCategory(args[4]);
                    }
                    if (args[3].ToLower() == "-p")
                    {
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Usage : filter -p [priority] ");
                            return;
                        }
                        taskService.FilterTasksByPriority(args[4]);

                    }

                    else
                    {
                        Console.WriteLine("Incorrect Usage");

                    }
                    break;

                case "sort":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Usage : sort asc/dec ");
                        return;
                    }
                    taskService.SortTasksByDueDate(args[3]);
                    break;

                

                default:
                    Console.WriteLine("Unknown command. Use add, view, complete, remove, or list.");
                    break;
            }
        }

       
    
    }
}
