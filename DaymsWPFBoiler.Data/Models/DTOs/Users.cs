using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Data.Models.DTOs
{
    public partial class Users
    {
        public Users()
        {

            UserRoles = new HashSet<UserRoles>();

        }

        // Base Props
        public Guid Id { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }

        // User Specific Props
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string IsAdmin { get; set; }

        public virtual ICollection<UserRoles> UserRoles { get; set; }

    }
}
