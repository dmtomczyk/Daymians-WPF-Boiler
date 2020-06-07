using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models;

namespace DaymsWPFBoiler.Models
{
    public class AnonymousIdentity : UserIdentity
    {
        public AnonymousIdentity() : base(new User()
        {
            Username = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty
        })
        { }
    }
}
