using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerCLI.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        public User(string username, string password, bool isAdmin)
        {
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
        }

       
        public static User Authenticate(string username, string password)
        {
            if (username == "admin" && password == "admin123")
                return new User(username, password, true);

            if (username == "user" && password == "user123")
                return new User(username, password, false);

            return null;
        }
    }
}
