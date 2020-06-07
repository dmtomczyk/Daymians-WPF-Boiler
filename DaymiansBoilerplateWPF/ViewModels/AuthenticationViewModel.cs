using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DaymsWPFBoiler.Data.Models;
using DaymsWPFBoiler.Data.Services.Interfaces;
using DaymsWPFBoiler.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NLog;

namespace DaymsWPFBoiler.ViewModels
{
    public class AuthenticationViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IAuthenticationService _authenticationService;
        private readonly ViewModelLocator _locator;

        private string _username;
        private string _status;

        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public bool IsAuthenticated => App.IsAuthenticated;

        public string AuthenticatedUser => IsAuthenticated ? string.Format("Signed in as {0}", Thread.CurrentPrincipal.Identity.Name) : "";

        public RelayCommand<object> LoginCommand { get; private set; }
        public RelayCommand<object> LogoutCommand { get; private set; }

        public AuthenticationViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _locator = Application.Current.FindResource("Locator") as ViewModelLocator;
            LoginCommand = new RelayCommand<object>(Login, CanLogin);
            LogoutCommand = new RelayCommand<object>(Logout, CanLogout);
        }

        private void Login(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                string clearTextPassword = passwordBox.Password ?? string.Empty;
                try
                {
                    // Validate credentials
                    User user = _authenticationService.AuthenticateUser(_locator.UnitOfWork, Username, clearTextPassword);

                    // Get current principal
                    if (!(Thread.CurrentPrincipal is UserPrincipal userPrincipal))
                    {
                        throw new ArgumentException("The application's default thread principal must be set to a UserPrincipal object on startup.");
                    }

                    // Autheticate user
                    userPrincipal.Identity = new UserIdentity(user);

                    // Refresh UI
                    RefreshUI();

                    // Reset fields
                    Username = string.Empty;
                    passwordBox.Password = string.Empty;

                    // Signal User Login
                    throw new NotImplementedException();
                    //Messenger.Default.Send(new NotificationMessage(UIStrings.Message_UserLogin));
                }
                catch (UnauthorizedAccessException)
                {
                    Status = "Login failed! Please provide some valid credentials.";
                }
                catch (Exception ex)
                {
                    Status = string.Format("ERROR: {0}", ex.Message);

                    Logger.Error(ex, "Unhandled Exception during Login()!\n");
                }
            }
        }

        private void RefreshUI()
        {
            RaisePropertyChanged(() => AuthenticatedUser);
            RaisePropertyChanged(() => IsAuthenticated);
            LoginCommand.RaiseCanExecuteChanged();
            LogoutCommand.RaiseCanExecuteChanged();
            Status = string.Empty;
        }

        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }

        private void Logout(object parameter)
        {

            try
            {
                if (Thread.CurrentPrincipal is UserPrincipal userPrincipal)
                {
                    userPrincipal.Identity = new AnonymousIdentity();
                    RefreshUI();

                    throw new NotImplementedException();
                    //Messenger.Default.Send(new NotificationMessage(UIStrings.Message_UserLogout));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ERROR DURING LOGOUT!\n");
            }
        }

        private bool CanLogout(object parameter)
        {
            return IsAuthenticated;
        }

        public async Task<string> GetServerAuthenticationTokenAsync()
        {
            string authenticationAttempt = null;
            if (IsAuthenticated && Thread.CurrentPrincipal.Identity is UserIdentity identity)
            {
                authenticationAttempt = await _authenticationService.GetServerAuthenticationTokenAsync(_locator.UnitOfWork, identity.User.Username);
            }
            return authenticationAttempt;
        }

        public async Task<DateTime> GetLastServerSyncTimeAsync()
        {
            DateTime result = DateTime.MinValue;
            if (IsAuthenticated && Thread.CurrentPrincipal.Identity is UserIdentity identity)
            {
                result = await _authenticationService.GetLastServerSyncTimeAsync(_locator.UnitOfWork, identity.User.Username);
            }
            return result;
        }

    }
}
