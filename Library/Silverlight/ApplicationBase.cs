using System;
using System.Text;
using System.Windows;

namespace Jellyfish.Library
{
    public class ApplicationBase : Application
    {
        public ApplicationBase(string name = null)
        {
            Name = name;

            UnhandledException += OnApplicationUnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        }

        private void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMessage(e.ExceptionObject), GetExceptionCaption("Application Exception"), MessageBoxButton.OK);
            e.Handled = true;
        }

        //private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show(GetExceptionMessage(e.ExceptionObject as Exception), GetExceptionCaption("AppDomain Exception", e.IsTerminating), MessageBoxButton.OK);
        //}

        private string GetExceptionCaption(string title, bool isTerminating = false)
        {
            var caption = new StringBuilder();
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
