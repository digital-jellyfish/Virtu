using System;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Jellyfish.Library
{
    public abstract class ApplicationBase : Application
    {
        protected ApplicationBase() : 
            this(null)
        {
        }

        protected ApplicationBase(string name)
        {
            Name = name;

            UnhandledException += OnApplicationUnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

            if (Debugger.IsAttached)
            {
                var settings = Application.Current.Host.Settings;
                settings.EnableFrameRateCounter = true;
                //settings.EnableRedrawRegions = true;
                //settings.EnableCacheVisualization = true;
            }
        }

        protected void InitializeOutOfBrowserUpdate()
        {
            if (IsRunningOutOfBrowser)
            {
                CheckAndDownloadUpdateCompleted += OnApplicationCheckAndDownloadUpdateCompleted;
                CheckAndDownloadUpdateAsync();
            }
        }

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

        private void OnApplicationCheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is PlatformNotSupportedException)
                {
                    MessageBox.Show("An application update is available, but it requires the latest version of Silverlight.");
                }
                //else if (Debugger.IsAttached)
                //{
                //    Debugger.Break();
                //}
            }
            else if (e.UpdateAvailable)
            {
                MessageBox.Show("An application update was downloaded. Restart the application to run the latest version.");
            }
        }

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

        public string Name { get; private set; }
    }
}
