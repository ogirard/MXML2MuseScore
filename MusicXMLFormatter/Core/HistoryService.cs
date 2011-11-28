using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Practices.Prism.ViewModel;

namespace MusicXMLFormatter.Core
{
  public class HistoryService : NotificationObject
  {
    #region    Singleton

    private static readonly HistoryService _instance = new HistoryService();

    private HistoryService()
    {
      HistoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data\\history.xml";
      Initialize();
    }

    public static HistoryService Instance
    {
      get { return _instance; }
    }

    #endregion Singleton

    public readonly string HistoryPath;

    private bool _isUpdating;

    private ObservableCollection<HistoryEntry> _history;

    public ObservableCollection<HistoryEntry> History
    {
      get { return _history; }
      set
      {
        if (_history != value)
        {
          if (_history != null)
          {
            _history.CollectionChanged -= HistoryChangedHandler;
          }

          _history = value;

          if (_history != null)
          {
            _history.CollectionChanged += HistoryChangedHandler;
          }

          RaisePropertyChanged(() => History);
        }
      }
    }

    private void HistoryChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.SaveHistory();
    }

    public void Add(ScoreDocument doc)
    {
      _isUpdating = true;
      var historyEntry = new HistoryEntry(doc);
      foreach (var olderEntry in History.Where(e => e.Title == historyEntry.Title).ToList())
      {
        History.Remove(olderEntry);
      }

      History.Add(historyEntry);
      _isUpdating = false;

      SaveHistory();
    }

    private void Initialize()
    {
      _isUpdating = true;
      History = new ObservableCollection<HistoryEntry>();
      var historyDoc = XDocument.Load(HistoryPath);
      foreach (var entry in historyDoc.Root.Elements("entry"))
      {
        _history.Add(new HistoryEntry(entry));
      }
      _isUpdating = false;
    }

    private void SaveHistory()
    {
      if (_isUpdating)
      {
        return;
      }

      var doc = XDocument.Load(HistoryPath);
      foreach (var child in doc.Root.Elements())
      {
        child.Remove();
      }

      foreach (var historyEntry in History)
      {
        doc.Root.Add(historyEntry.ToXmlNode());
      }

      doc.Save(HistoryPath);
    }

    public void Remove(HistoryEntry historyEntry)
    {
      History.Remove(historyEntry);
    }
  }
}