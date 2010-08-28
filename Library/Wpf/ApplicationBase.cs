using System;
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Jellyfish.Library
{
    public class ApplicationBase : Application
    {
        [SecurityCritical]
        public ApplicationBase() : 
            this(null)
        {
        }

        [SecurityCritical]
        public ApplicationBase(string name)
        {
            Name = name;

            DispatcherUnhandledException += OnApplicationDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
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

        private void OnApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), GetExceptionCaption("Application Dispatcher Exception", isTerminating: true));
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            e.Handled = true;
            Shutdown();
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), GetExceptionCaption("AppDomain Exception", e.IsTerminating));
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        public string Name { get; private set; }
    }
}
