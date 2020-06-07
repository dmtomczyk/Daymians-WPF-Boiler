using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models;
using DaymsWPFBoiler.Data.Models.DTOs;

namespace DaymsWPFBoiler.Data.Utilities
{
    public static class AuthFunctions
    {
        public static string CalculateHash(string clearTextPassword, string salt)
        {
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            return Convert.ToBase64String(hash);
        }

        public static User ConvertUser(Users u)
        {
            return new User()
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailAddress = u.EmailAddress,
                IsAdmin = bool.Parse(u.IsAdmin ?? "false"),
                Roles = new ObservableCollection<Role>(u.UserRoles.Select(r => ConvertRole(r.Role)))
            };
        }

        public static Users ConvertUser(User u)
        {
            Users user = new Users()
            {
                Id = u.Id,
                Username = u.Username,
                Password = CalculateHash(u.Username, u.Username),
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailAddress = u.EmailAddress,
                IsAdmin = u.IsAdmin.ToString()
            };

            return user;
        }

        public static Role ConvertRole(Roles r)
        {
            return new Role()
            {
                Id = r.Id,
                Name = r.Name
            };
        }

        public static Roles ConvertRole(Role r)
        {
            return new Roles()
            {
                Id = r.Id,
                Name = r.Name
            };
        }

        public static void CopyUser(User source, Users target)
        {

            target.Id = source.Id;
            target.Username = source.Username;
            target.Password = CalculateHash(source.Username, source.Username);
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.EmailAddress = source.EmailAddress;
            target.IsAdmin = source.IsAdmin.ToString();

        }


    }
}
