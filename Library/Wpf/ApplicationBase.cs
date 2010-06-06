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

        private void OnApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMessage(e.Exception), GetExceptionCaption("Application Dispatcher Exception", isTerminating: true));
            e.Handled = true;
            Shutdown();
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMessage(e.ExceptionObject as Exception), GetExceptionCaption("AppDomain Exception", e.IsTerminating));
        }

        private string GetExceptionCaption(string title, bool isTerminating = false)
        {
            var caption = new StringBuilder();
            caption.AppendFormat("[{0}] ", Process.GetCurrentProcess().Id);
            if (!string.IsNullOrEmpty(Name))
            {
                caption.Append(Name);
                caption.Append(" ");
            }
            caption.Append(title);
            if (isTerminating)
            {
                caption.Append(" (Terminating)");
            }

            return caption.ToString();
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var message = new StringBuilder();
            if (exception != null)
            {
                message.Append(exception.Message.ToString());
                message.Append(Environment.NewLine);
                message.Append(exception.StackTrace.ToString());
            }

            return message.ToString();
        }

        public string Name { get; private set; }
    }
}
