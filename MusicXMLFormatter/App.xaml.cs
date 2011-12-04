using System;
using System.Runtime.InteropServices;
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

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      if (e.Args.Length == 11)
      {
        WriteToConsole("Called with args:");
        for (int i = 0; i < e.Args.Length; i++)
        {
          WriteToConsole((i + 1) + ": " + e.Args[i]);
        }

         ((MainWindow)Current.MainWindow).ViewModel.LoadMusicXMLFileCommand.Execute();

        var doc = new ScoreDocument(e.Args[0]);
        doc.Title = e.Args[1];
        doc.SubTitle = e.Args[2];
        doc.Pattern = int.Parse(e.Args[3]);
        doc.Composer = e.Args[4];
        doc.Texter = e.Args[5];
        doc.ArrangedBy = e.Args[6];
        doc.ExportPDF = e.Args[7] == "True";
        doc.ExportPNG = e.Args[8] == "True";
        doc.ExtractVoice = e.Args[9] == "True";
        doc.RemoveLabels = e.Args[10] == "True";
      }
    }

    public void WriteToConsole(string message)
    {
      _connected = _connected || AttachConsole(-1);
      if (_connected)
      {
        Console.WriteLine(message);
      }
    }

    bool _connected;

    [DllImport("Kernel32.dll")]
    public static extern bool AttachConsole(int processId);
  }
}
