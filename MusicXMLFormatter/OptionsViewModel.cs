using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MusicXMLFormatter.Properties;

namespace MusicXMLFormatter
{
  public class OptionsViewModel : NotificationObject
  {
    public OptionsViewModel()
    {
      SaveCommand = new DelegateCommand(SaveOptions);
      _outputPath = Settings.Default.OutputPath;
      _musePath = Settings.Default.MuseScoreExe;
    }

    private void SaveOptions()
    {
      Settings.Default.OutputPath = OutputPath;
      Settings.Default.MuseScoreExe = MusePath;
      Settings.Default.Save();
    }

    private string _outputPath;

    public string OutputPath
    {
      get { return this._outputPath; }
      set
      {
        if (this._outputPath != value)
        {
          this._outputPath = value;
          RaisePropertyChanged(() => this.OutputPath);
        }
      }
    }

    private string _musePath;

    public string MusePath
    {
      get { return this._musePath; }
      set
      {
        if (this._musePath != value)
        {
          this._musePath = value;
          RaisePropertyChanged(() => this.MusePath);
        }
      }
    }

    public ICommand SaveCommand { get; set; }
  }
}