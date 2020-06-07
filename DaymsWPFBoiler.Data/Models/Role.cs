using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Data.Models
{
    public class Role : BaseEntity
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public static bool operator ==(Role obj1, Role obj2)
        {
            return ReferenceEquals(obj1, obj2) ? true : obj1 is null || obj2 is null ? false : obj1.Name == obj2.Name;
        }

        public static bool operator !=(Role obj1, Role obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            return obj is null ? false : ReferenceEquals(this, obj) ? true : obj is Role role && role.Name == Name;
        }

        public override int GetHashCode()
        {
            int hashCode = -858915621;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
