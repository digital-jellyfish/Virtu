using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Jellyfish.Library
{
    public class ApplicationBase : Application
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public ApplicationBase(string name)
        {
            Name = name;

            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMessage(e.Exception), GetExceptionCaption("Application Dispatcher Exception", true));
            Shutdown();
            e.Handled = true;
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMessage(e.ExceptionObject as Exception), GetExceptionCaption("AppDomain Exception", e.IsTerminating));
        }

        private string GetExceptionCaption(string title, bool isTerminating)
        {
            StringBuilder caption = new StringBuilder();
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
            StringBuilder message = new StringBuilder();
            if (exception != null)
            {
                message.Append(exception.Message.ToString());
                message.Append(Environment.NewLine);
                message.Append(Environment.NewLine);
                message.Append(exception.ToString()); // includes stack trace
            }

            return message.ToString();
        }

        public string Name { get; private set; }
    }
}
