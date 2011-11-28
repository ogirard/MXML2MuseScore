using System.Windows;
using System.Windows.Input;

namespace MusicXMLFormatter
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class Options : Window
  {
    public Options()
    {
      InitializeComponent();
      ViewModel = new OptionsViewModel();
    }

    public OptionsViewModel ViewModel
    {
      get { return DataContext as OptionsViewModel; }
      set { DataContext = value; }
    }

    private void SaveClickHandler(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
