using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DaymsWPFBoiler.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DaymsWPFBoiler.ViewModels
{
    public class UserMenuViewModel : ViewModelBase
    {
        private string _displayName;
        private string _shortName;
        private bool _isAuthenticated;
        private bool _isAdmin;

        public string DisplayName { get => _displayName; private set => Set(ref _displayName, value); }
        public string ShortName { get => _shortName; private set => Set(ref _shortName, value); }
        public bool IsAuthenticated { get => _isAuthenticated; private set => Set(ref _isAuthenticated, value); }
        public bool IsAdmin { get => _isAdmin; private set => Set(ref _isAdmin, value); }

        public ICommand AboutMeCommand => new RelayCommand(() =>
        {
            //MessageBox.Show("\"About me\" clicked!");
        });

        public ICommand AccountSettingsCommand => new RelayCommand(() =>
        {
            //MessageBox.Show("\"Account settings\" clicked!");
        });

        public ICommand SwitchAccountCommand => new RelayCommand(() =>
        {
            //Messenger.Default.Send(new NotificationMessage(UIStrings.Message_Open_UserLogin));
        });

        public ICommand ManageUsersCommand => new RelayCommand(() =>
        {
            //Messenger.Default.Send(new NotificationMessage(UIStrings.Message_Open_UserManagement));
        });

        internal void Refresh()
        {
            if (Thread.CurrentPrincipal is UserPrincipal principal && principal.Identity is UserIdentity identity)
            {
                DisplayName = identity.Name;
                ShortName = identity.ShortName;
                IsAuthenticated = identity.IsAuthenticated;
                IsAdmin = identity.User.IsAdmin;
            }
        }

        public UserMenuViewModel()
        {
            Refresh();
        }
    }
}
