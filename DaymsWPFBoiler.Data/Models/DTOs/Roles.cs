using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Data.Models.DTOs
{
    public partial class Roles
    {
        public Roles()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public Guid Id { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}
