using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Data.Models.DTOs
{
    public partial class UserRoles
    {
        public Guid Id { get; set; }
        public string DateAssigned { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public virtual Roles Role { get; set; }
        public virtual Users User { get; set; }
    }
}
