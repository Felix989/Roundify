using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Input;

namespace Roundify
{
    /// <summary>
    /// Closes the current window.
    /// </summary>
    public class CloseWindowCommand : MarkupExtension, ICommand
    {
        public void Execute(object parameter)
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
            CommandManager.InvalidateRequerySuggested();
        }

        public event EventHandler CanExecuteChanged;


        public bool CanExecute(object parameter)
        {
            Window win = Application.Current.MainWindow;
            return win != null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}