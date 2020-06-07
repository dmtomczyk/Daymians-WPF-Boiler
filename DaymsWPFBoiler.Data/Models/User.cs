using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace DaymsWPFBoiler.Data.Models
{
    public class User : BaseEntity
    {
        private string _username;
        [Required]
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        private string _firstName;
        [Required]
        public string FirstName
        {
            get => _firstName;
            set => Set(ref _firstName, value);
        }

        private string _lastName;
        [Required]
        public string LastName
        {
            get => _lastName;
            set => Set(ref _lastName, value);
        }

        private ObservableCollection<Role> _roles;

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set => Set(ref _roles, value);
        }

        private bool _isAdmin;

        public bool IsAdmin
        {
            get => _isAdmin;
            set => Set(ref _isAdmin, value);
        }

        private string _emailAddress;

        [EmailAddress]
        public string EmailAddress
        {
            get => _emailAddress;
            set => Set(ref _emailAddress, value);
        }

        public User()
        {
            Roles = new ObservableCollection<Role>();
        }

    }
}
