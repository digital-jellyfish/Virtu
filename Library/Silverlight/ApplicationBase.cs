using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
#if WINDOWS_PHONE
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
#endif

namespace Jellyfish.Library
{
    public class ApplicationBase : Application
    {
        public ApplicationBase() : 
            this(null)
        {
        }

        public ApplicationBase(string name)
        {
            Name = name;

            UnhandledException += OnApplicationUnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        }

#if !WINDOWS_PHONE
        protected void InitializeOutOfBrowserUpdate()
        {
            if (IsRunningOutOfBrowser)
            {
                CheckAndDownloadUpdateCompleted += OnApplicationCheckAndDownloadUpdateCompleted;
                CheckAndDownloadUpdateAsync();
            }
        }
#endif

#if WINDOWS_PHONE
        protected void InitializePhoneApplication()
        {
            if (!_phoneApplicationInitialized)
            {
                RootFrame = new PhoneApplicationFrame();
                RootFrame.Navigated += OnRootFrameNavigated;
                RootFrame.NavigationFailed += OnRootFrameNavigationFailed;
                _phoneApplicationInitialized = true;
            }
        }
#endif

        private string GetExceptionCaption(string title, bool isTerminating = false)
        {
            var caption = new StringBuilder();
            if (!string.IsNullOrEmpty(Name))
            {
                caption.Append(Name).Append(' ');
            }
            caption.Append(title);
            if (isTerminating)
            {
                caption.Append(" (Terminating)");
            }

            return caption.ToString();
        }

#if !WINDOWS_PHONE
        private void OnApplicationCheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is PlatformNotSupportedException)
                {
                    MessageBox.Show("An application update is available, but it requires the latest version of Silverlight.");
                }
                else if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
            else if (e.UpdateAvailable)
            {
                MessageBox.Show("An application update was downloaded. Restart the application to run the latest version.");
            }
        }
#endif

        private void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), GetExceptionCaption("Application Exception"), MessageBoxButton.OK);
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        //private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show(e.ExceptionObject.ToString(), GetExceptionCaption("AppDomain Exception", e.IsTerminating), MessageBoxButton.OK);
        //    if (Debugger.IsAttached)
        //    {
        //        Debugger.Break();
        //    }
        //}

#if WINDOWS_PHONE
        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (RootVisual != RootFrame)
            {
                RootVisual = RootFrame;
            }
            RootFrame.Navigated -= OnRootFrameNavigated;
        }

        private void OnRootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
#endif

        public string Name { get; private set; }
#if WINDOWS_PHONE
        public PhoneApplicationFrame RootFrame { get; private set; }

        private bool _phoneApplicationInitialized;
#endif
    }
}
