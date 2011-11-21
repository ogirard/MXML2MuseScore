using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;

namespace MusicXMLFormatter
{
    public class MainWindowViewModel : NotificationObject
    {
        private MusicXMLDocument _currentDocument;
        private string _loadedDocument;

        public DelegateCommand LoadMusicXMLFileCommand { get; set; }
        public DelegateCommand ConvertMusicXMLFileCommand { get; set; }
        public DelegateCommand SaveCurrentDocumentCommand { get; set; }

        public MainWindowViewModel()
        {
            LoadMusicXMLFileCommand = new DelegateCommand(LoadMusicXMLFile, () => !IsBusy);
            ConvertMusicXMLFileCommand = new DelegateCommand(ConvertMusicXMLFile, () => IsEditingAllowed);
            SaveCurrentDocumentCommand = new DelegateCommand(SaveCurrentDocument, () => IsEditingAllowed);
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
            get { return _loadedDocument ?? "Keine MusicXML Datei geladen"; }
            private set
            {
                if (_loadedDocument != value)
                {
                    _loadedDocument = value;
                    RaisePropertyChanged(() => LoadedDocument);
                }
            }
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
                });
            }
        }

        public bool IsEditingAllowed
        {
            get { return CurrentDocument != null && !IsBusy; }
        }

        public MusicXMLDocument CurrentDocument
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
                    LoadedDocument = CurrentDocument == null ? null : CurrentDocument.FileName;
                }
            }
        }

        private void LoadMusicXMLFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Music XML Dateien|*.xml";
            if ((bool)ofd.ShowDialog())
            {
                IsBusy = true;
                BusyText = "Lade Datei...";
                var task = Task.Factory.StartNew(() => new MusicXMLDocument(ofd.FileName));
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

        private bool _isBusy;

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
    }
}