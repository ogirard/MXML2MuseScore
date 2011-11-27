using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MusicXMLFormatter.Core;
using MusicXMLFormatter.Properties;

namespace MusicXMLFormatter
{
  public class OptionsViewModel : NotificationObject
  {
    public OptionsViewModel()
    {
      SaveCommand = new DelegateCommand(SaveOptions);
      DeleteEntryCommand = new DelegateCommand(DeleteEntry, () => SelectedHistoryEntry != null);
      _outputPath = Settings.Default.OutputPath;
      _musePath = Settings.Default.MuseScoreExe;
    }

    private void DeleteEntry()
    {
      throw new System.NotImplementedException();
    }

    private void SaveOptions()
    {
      Settings.Default.OutputPath = OutputPath;
      Settings.Default.MuseScoreExe = MusePath;
      Settings.Default.Save();
      HistoryEntry.SaveHistory(History);
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

    private HistoryEntry _selectedHistoryEntry;

    public HistoryEntry SelectedHistoryEntry
    {
      get { return this._selectedHistoryEntry; }
      set
      {
        if (this._selectedHistoryEntry != value)
        {
          this._selectedHistoryEntry = value;
          RaisePropertyChanged(() => this.SelectedHistoryEntry);
          DeleteEntryCommand.RaiseCanExecuteChanged();
        }
      }
    }

    public ICommand SaveCommand { get; set; }
    public DelegateCommand DeleteEntryCommand { get; set; }
    public ObservableCollection<HistoryEntry> History { get; set; }
  }
}