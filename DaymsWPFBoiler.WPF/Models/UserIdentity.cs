using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models;

namespace DaymsWPFBoiler.Models
{
    public class UserIdentity : IIdentity
    {

        public User User { get; private set; }

        public string Name
        {
            get
            {
                string displayName = (User.FirstName ?? "") + " " + (User.LastName ?? "");
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = User.Username;
                }

                return displayName;
            }
        }

        public string ShortName
        {
            get
            {
                return Name == User.Username ? "^_^" : User.FirstName.Substring(0, 1) + User.LastName.Substring(0, 1);
            }
        }

        public string AuthenticationType { get { return "Daym's WPF Authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }

        public UserIdentity(User user)
        {
            User = user;
        }

    }
}
