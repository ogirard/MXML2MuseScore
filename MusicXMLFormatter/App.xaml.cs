using System;
using System.Windows;
using System.Windows.Threading;
using MusicXMLFormatter.Core;

namespace MusicXMLFormatter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      private void AppUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
      {
        if (e.Exception is HandledErrorException)
        {
          ((MainWindow)Current.MainWindow).ViewModel.HandledErrorException = e.Exception as HandledErrorException;
          e.Handled = true;
        }
      }
    }
}
