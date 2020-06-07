using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models;
using DaymsWPFBoiler.Data.UnitOfWork;

namespace DaymsWPFBoiler.Data.Services.Interfaces
{
    public interface IAuthenticationService
    {
        User AuthenticateUser(IUnitOfWork DaymsWork, string username, string password);
        ObservableCollection<User> GetUsers(IUnitOfWork DaymsWork);
        void UpsertUser(IUnitOfWork DaymsWork, User userToUpdate);
        bool DeleteUser(IUnitOfWork DaymsWork, User userToDelete);
        ObservableCollection<Role> GetRoles(IUnitOfWork DaymsWork);

        Task<string> GetServerAuthenticationTokenAsync(IUnitOfWork DaymsWork, string username);

        Task<DateTime> GetLastServerSyncTimeAsync(IUnitOfWork DaymsWork, string username);
    }
}
