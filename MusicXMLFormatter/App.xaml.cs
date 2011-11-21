using System;
using System.Windows;
using System.Windows.Threading;

namespace MusicXMLFormatter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      private void AppUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
      {
        
        throw new NotImplementedException();
      }
    }
}
