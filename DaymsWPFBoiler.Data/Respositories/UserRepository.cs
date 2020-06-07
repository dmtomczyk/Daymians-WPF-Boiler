using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Contexts;
using DaymsWPFBoiler.Data.Models.DTOs;
using DaymsWPFBoiler.Data.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DaymsWPFBoiler.Data.Respositories
{
    public class UserRepository : Repository<Users>, IUserRepository
    {
        public UserRepository(DaymsContext context) : base(context)
        {
        }

        public new Users Get(Guid id)
        {
            // Removing .ThenInclude(org => org.Sections) resolved the InvalidOperationException (tracking entityId)
            // Disabling tracking of sections also resolves the error.
            return DaymsContext.Users
                .Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role)
                .SingleOrDefault(u => u.Id == id);
        }

        public IEnumerable<Users> GetAdminUsers()
        {
            return DaymsContext.Users
                .Where(u => bool.Parse(u.IsAdmin));
        }

        public IEnumerable<Users> GetAllWithRoles()
        {
            List<Users> result = DaymsContext.Users
                .Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role)
                .ToList();

            return result;
        }

        public DaymsContext DaymsContext => Context as DaymsContext;
    }
}
