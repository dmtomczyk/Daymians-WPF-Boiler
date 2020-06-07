using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Models
{
    internal class UserPrincipal : IPrincipal
    {

        private UserIdentity _identity;

        public UserIdentity Identity { get => _identity ?? new AnonymousIdentity(); set => _identity = value; }

        IIdentity IPrincipal.Identity { get => Identity; }

        public bool IsInRole(string role)
        {
            return false;
        }

    }
}
