using System;
using System.Collections.Generic;
using System.Linq;
using MinimalApiAuth.Models;

namespace MinimalApiAuth.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>()
            {
                new User() { Id = 1, UserName = "batman", Password = "batman", Role = "manager" },
                new User() { Id = 2, UserName = "robin", Password = "robin", Role = "employee" }
            };
            return users.FirstOrDefault(x =>
                string.Equals(x.UserName, username, StringComparison.CurrentCultureIgnoreCase) 
                && x.Password.ToLower() == password.ToLower());
        }

    }
}