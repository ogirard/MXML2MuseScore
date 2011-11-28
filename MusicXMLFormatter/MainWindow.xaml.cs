using System.Windows;

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
  }
}
