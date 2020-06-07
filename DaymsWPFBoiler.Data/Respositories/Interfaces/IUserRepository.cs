using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models.DTOs;

namespace DaymsWPFBoiler.Data.Respositories.Interfaces
{
    public interface IUserRepository : IRepository<Users>
    {
        IEnumerable<Users> GetAdminUsers();
        IEnumerable<Users> GetAllWithRoles();
    }
}
