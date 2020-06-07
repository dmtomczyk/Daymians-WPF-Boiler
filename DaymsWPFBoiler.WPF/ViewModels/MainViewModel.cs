using System.Collections.ObjectModel;
using System.Windows;
using DaymsWPFBoiler.Data.Services.Interfaces;
using DaymsWPFBoiler.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using NLog;

namespace DaymsWPFBoiler.ViewModels
{
    /// <summary>
    /// This class contains properties that MainWindow.xaml can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            IDataService service,
            UserMenuViewModel userMenuViewModel,
            AuthenticationViewModel authenticationViewModel)
        {
            _service = service;
            _locator = Application.Current.FindResource("Locator") as ViewModelLocator;


        }

        /// <summary>
        /// Assigned to in the constructor via Dependency Injection.
        /// </summary>
        public readonly IDataService _service;

        /// <summary>
        /// Used to access various ViewModels as needed.
        /// </summary>
        public readonly ViewModelLocator _locator;

        /// <summary>
        /// NLog's Logger. Used to log errors, other stuff to .txt.
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<IDialogViewModel> _dialogs = new ObservableCollection<IDialogViewModel>();
        /// <summary>
        /// Container for all 'Dialogs' and popup windows for this application.
        /// </summary>
        public ObservableCollection<IDialogViewModel> Dialogs
        {
            get => _dialogs;
            set => Set(ref _dialogs, value);
        }

    }
}