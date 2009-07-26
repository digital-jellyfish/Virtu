using System;
using System.Text;
using System.Windows;

namespace Jellyfish.Library
{
    public class ApplicationBase : Application
    {
        public ApplicationBase(string name)
        {
            Name = name;

            UnhandledException += Application_UnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMessage(e.ExceptionObject), GetExceptionCaption("Application Exception", false), MessageBoxButton.OK);
        }

        //private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show(GetExceptionMessage(e.ExceptionObject as Exception), GetExceptionCaption("AppDomain Exception", e.IsTerminating), MessageBoxButton.OK);
        //}

        private string GetExceptionCaption(string title, bool isTerminating)
        {
            StringBuilder caption = new StringBuilder();
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
