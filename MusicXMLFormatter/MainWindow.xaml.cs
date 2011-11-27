using System.Windows;
using System.Windows.Controls;
using MusicXMLFormatter.Core;

namespace MusicXMLFormatter
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      ViewModel = new MainWindowViewModel();
    }

    public MainWindowViewModel ViewModel
    {
      get { return DataContext as MainWindowViewModel; }
      set { DataContext = value; }
    }

    private void HistoryEntryChangedHandler(object sender, SelectionChangedEventArgs e)
    {
      if (ViewModel.CurrentDocument != null)
      {
        foreach (var item in e.AddedItems)
        {
          if (item is HistoryEntry)
          {
            ((HistoryEntry)item).ApplyScoreDocument(ViewModel.CurrentDocument);
          }
        }
      }
    }
  }
}
