using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DaymsWPFBoiler.Models;

namespace DaymsWPFBoiler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsAuthenticated => Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? false;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Create a UserPrincipal with AnonymousIdentity at startup
            UserPrincipal userPrincipal = new UserPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(userPrincipal);

            // Setting MainWindow as StartupWindow
            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

            base.OnStartup(e);
        }


    }
}
