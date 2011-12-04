using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using MusicXMLFormatter.Core;

namespace MusicXMLFormatter
{
  public class MainWindowViewModel : NotificationObject
  {
    private readonly HistoryService _historyService;
    private ScoreDocument _currentDocument;
    private string _loadedDocument;

    public DelegateCommand LoadMusicXMLFileCommand { get; set; }
    public DelegateCommand ConvertMusicXMLFileCommand { get; set; }
    public DelegateCommand SaveCurrentDocumentCommand { get; set; }
    public DelegateCommand ShowOptionsCommand { get; set; }
    public DelegateCommand<SelectionChangedEventArgs> HistorySelectionChangedCommand { get; set; }

    public MainWindowViewModel()
    {
      _historyService = HistoryService.Instance;
      LoadMusicXMLFileCommand = new DelegateCommand(LoadMusicXMLFile, () => !IsBusy);
      ConvertMusicXMLFileCommand = new DelegateCommand(ConvertMusicXMLFile, () => IsEditingAllowed);
      SaveCurrentDocumentCommand = new DelegateCommand(SaveCurrentDocument, () => IsEditingAllowed);
      ShowOptionsCommand = new DelegateCommand(ShowOptions);
      HistorySelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(HistorySelectionChangedHandler, e => CurrentDocument != null);
    }

    public ObservableCollection<HistoryEntry> History
    {
      get { return _historyService.History; }
    }

    private void ShowOptions()
    {
      Options options = new Options();
      options.ShowDialog();
    }

    private void ConvertMusicXMLFile()
    {
      IsBusy = true;
      BusyText = "Konvertiere Datei...";
      var task = Task.Factory.StartNew(() =>
                                           {
                                             var museScore = new MuseScoreApp();
                                             museScore.ConvertXMLtoMuseScore(CurrentDocument.FileName);
                                           });
      task.ContinueWith(resultTask =>
      {
        IsBusy = false;
      });
    }

    public string LoadedDocument
    {
      get { return ShortenFileName(_loadedDocument) ?? "Keine MusicXML Datei geladen"; }
      private set
      {
        if (_loadedDocument != value)
        {
          _loadedDocument = value;
          RaisePropertyChanged(() => LoadedDocument);
        }
      }
    }

    private static string ShortenFileName(string fileName)
    {
      const int MaxLength = 50;
      if (fileName == null || fileName.Length <= MaxLength)
      {
        return fileName;
      }

      var name = Path.GetFileName(fileName) ?? "";
      var firstPart = fileName.Substring(0, fileName.Length - name.Length - 1);
      var shortPath = firstPart.Substring(Math.Max(firstPart.Length - MaxLength - name.Length - 3, 0)) + "\\" + name;
      shortPath = shortPath.Substring(shortPath.IndexOf('\\') > 0 ? shortPath.IndexOf('\\') : 0);
      if (shortPath.Length + fileName.Length > MaxLength + 20)
      {
        shortPath = name;
      }

      return "..." + shortPath;
    }

    private void SaveCurrentDocument()
    {
      if (_currentDocument != null)
      {
        IsBusy = true;
        BusyText = "Speichere Datei...";
        var task = Task.Factory.StartNew(() => _currentDocument.Save());
        task.ContinueWith(resultTask =>
        {
          IsBusy = false;

          // update history
          Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _historyService.Add(_currentDocument)));
        });
      }
    }

    public bool IsEditingAllowed
    {
      get { return CurrentDocument != null && !IsBusy; }
    }

    public ScoreDocument CurrentDocument
    {
      get { return _currentDocument; }
      set
      {
        if (_currentDocument != value)
        {
          _currentDocument = value;
          RaisePropertyChanged(() => CurrentDocument);
          RaisePropertyChanged(() => this.IsEditingAllowed);
          SaveCurrentDocumentCommand.RaiseCanExecuteChanged();
          ConvertMusicXMLFileCommand.RaiseCanExecuteChanged();
          ShowOptionsCommand.RaiseCanExecuteChanged();
          HistorySelectionChangedCommand.RaiseCanExecuteChanged();
          LoadedDocument = CurrentDocument == null ? null : CurrentDocument.FileName;
        }
      }
    }

    private void LoadMusicXMLFile()
    {
      OpenFileDialog ofd = new OpenFileDialog { Filter = "Music XML Dateien|*.xml" };
      if ((bool)ofd.ShowDialog())
      {
        IsBusy = true;
        BusyText = "Lade Datei...";
        var task = Task.Factory.StartNew(() => new ScoreDocument(ofd.FileName));
        task.ContinueWith(resultTask =>
                              {
                                CurrentDocument = resultTask.Result;
                                IsBusy = false;
                              });
      }
    }

    private string _busyText = string.Empty;

    public string BusyText
    {
      get { return this._busyText; }
      set
      {
        if (this._busyText != value)
        {
          this._busyText = value;
          RaisePropertyChanged(() => this.BusyText);
        }
      }
    }

    private void HistorySelectionChangedHandler(SelectionChangedEventArgs e)
    {
      foreach (HistoryEntry item in e.AddedItems.OfType<HistoryEntry>())
      {
        item.ApplyToScoreDocument(CurrentDocument);
      }
    }

    private bool _isBusy;
    private HandledErrorException _handledErrorException;

    public bool IsBusy
    {
      get { return this._isBusy; }
      set
      {
        if (this._isBusy != value)
        {
          this._isBusy = value;
          RaisePropertyChanged(() => this.IsBusy);
          LoadMusicXMLFileCommand.RaiseCanExecuteChanged();
          RaisePropertyChanged(() => this.IsEditingAllowed);
          SaveCurrentDocumentCommand.RaiseCanExecuteChanged();
          ConvertMusicXMLFileCommand.RaiseCanExecuteChanged();
        }
      }
    }

    public HandledErrorException HandledErrorException
    {
      get
      {
        return _handledErrorException;
      }

      set
      {
        if (_handledErrorException != value)
        {
          _handledErrorException = value;
          this.RaisePropertyChanged(() => HandledErrorException);
        }
      }
    }
  }
}