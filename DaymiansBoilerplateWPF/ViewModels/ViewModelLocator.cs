/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DaymiansBoilerplateWPF"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using DaymsWPFBoiler.Data.Services;
using DaymsWPFBoiler.Data.Services.Interfaces;
using DaymsWPFBoiler.Data.UnitOfWork;
using GalaSoft.MvvmLight.Ioc;

namespace DaymsWPFBoiler.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IAuthenticationService, BasicAuthenticationService>();
            SimpleIoc.Default.Register<IDataService, BasicDataService>();
            SimpleIoc.Default.Register<IUnitOfWork, UnitOfWork>();

            SimpleIoc.Default.Register<MainViewModel>();

            // Registers for USER RELATED ViewModels
            SimpleIoc.Default.Register<AuthenticationViewModel>();
            SimpleIoc.Default.Register<UserMenuViewModel>();

        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public UserMenuViewModel UserMenu => ServiceLocator.Current.GetInstance<UserMenuViewModel>();

        public AuthenticationViewModel Authentication => ServiceLocator.Current.GetInstance<AuthenticationViewModel>();

        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (SimpleIoc.Default.ContainsCreated<IUnitOfWork>())
                {
                    SimpleIoc.Default.Unregister<IUnitOfWork>();
                    SimpleIoc.Default.Register<IUnitOfWork, UnitOfWork>();
                }
                return ServiceLocator.Current.GetInstance<IUnitOfWork>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}