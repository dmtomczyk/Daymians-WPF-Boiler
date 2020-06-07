using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models;
using DaymsWPFBoiler.Data.Models.DTOs;
using DaymsWPFBoiler.Data.Services.Interfaces;
using DaymsWPFBoiler.Data.UnitOfWork;
using DaymsWPFBoiler.Data.Utilities;

namespace DaymsWPFBoiler.Data.Services
{
    public class BasicAuthenticationService : IAuthenticationService
    {
        // Initializing the logger in this class.
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ObservableCollection<User> GetUsers(IUnitOfWork DaymsWork)
        {
            using (DaymsWork)
            {
                return new ObservableCollection<User>(DaymsWork.Users.GetAllWithRoles().Select(u => AuthFunctions.ConvertUser(u)));
            }
        }
        
        public User AuthenticateUser(IUnitOfWork DaymsWork, string username, string password)
        {
            using (DaymsWork)
            {
                string hashedPassword = AuthFunctions.CalculateHash(password, username);
                if (!(DaymsWork.Users.SingleOrDefault(u => u.Username == username && u.Password == hashedPassword) is Users userData))
                {
                    throw new UnauthorizedAccessException("Access denied! Please provide some valid credentials.");
                }

                return AuthFunctions.ConvertUser(userData);
            }
        }

        public void UpsertUser(IUnitOfWork DaymsWork, User user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(IUnitOfWork DaymsWork, User user)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Role> GetRoles(IUnitOfWork DaymsWork)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Authentication Token for User from Guacamole Server
        /// </summary>
        /// <param name="DaymsWork">Unit of Work for this process</param>
        /// <param name="username">The username of the User requesting Authentication Token</param>
        /// <returns>The Authentication Token from the Server, or an error message.</returns>
        public async Task<string> GetServerAuthenticationTokenAsync(IUnitOfWork DaymsWork, string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get DateTime of Last Synchronization with Guacamole Server for User
        /// </summary>
        /// <param name="DaymsWork">Unit of Work for this process</param>
        /// <param name="username">The username of the User requesting Authentication Token</param>
        /// <returns>The Authentication Token from the Server, or an error message.</returns>
        public async Task<DateTime> GetLastServerSyncTimeAsync(IUnitOfWork DaymsWork, string username)
        {
            throw new NotImplementedException();
        }

    }
}
