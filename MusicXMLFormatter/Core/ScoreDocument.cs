using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Practices.Prism.ViewModel;
using MusicXMLFormatter.Properties;
using MusicXMLFormatter.Transformer;

namespace MusicXMLFormatter.Core
{
  public class ScoreDocument : NotificationObject
  {
    private static readonly IList<IMuseScoreTransformer> Transformers = new List<IMuseScoreTransformer>();
    private const string ComposerPrefix = "M: ";
    private const string TexterPrefix = "T: ";
    private const string ArrangedByPrefix = "arr. ";
    private const string PatternPrefix = " (Begleitvariante ";

    private const string FileNamePostfix = "_Begleitvariante";

    static ScoreDocument()
    {
      Transformers.Add(new ArrangedByTransformer());
      Transformers.Add(new RemoveCaptionsTransformer());
    }

    private readonly string _fileName;

    public ScoreDocument(string fileName)
    {
      this._fileName = fileName;
      this.LoadFromXml();
    }

    #region    Properties

    public string FileName
    {
      get { return _fileName; }
    }

    private string _title = string.Empty;

    public string Title
    {
      get { return _title; }
      set
      {
        if (_title != value)
        {
          _title = value;
          RaisePropertyChanged(() => Title);
        }
      }
    }

    private string _subTitle = string.Empty;

    public string SubTitle
    {
      get { return _subTitle; }
      set
      {
        if (_subTitle != value)
        {
          _subTitle = value;
          RaisePropertyChanged(() => SubTitle);
        }
      }
    }

    private string _composer = string.Empty;

    public string Composer
    {
      get { return _composer; }
      set
      {
        if (_composer != value)
        {
          _composer = value;
          RaisePropertyChanged(() => Composer);
        }
      }
    }

    private int _pattern;

    public int Pattern
    {
      get { return _pattern; }
      set
      {
        if (_pattern != value)
        {
          _pattern = value;
          RaisePropertyChanged(() => Pattern);
        }
      }
    }

    private bool _exportPNG;

    public bool ExportPNG
    {
      get { return _exportPNG; }
      set
      {
        if (_exportPNG != value)
        {
          _exportPNG = value;
          RaisePropertyChanged(() => ExportPNG);
        }
      }
    }

    private bool _exportPDF;

    public bool ExportPDF
    {
      get { return _exportPDF; }
      set
      {
        if (_exportPDF != value)
        {
          _exportPDF = value;
          RaisePropertyChanged(() => ExportPDF);
        }
      }
    }

    private bool _extractVoice;

    public bool ExtractVoice
    {
      get { return _extractVoice; }
      set
      {
        if (_extractVoice != value)
        {
          _extractVoice = value;
          RaisePropertyChanged(() => ExtractVoice);
        }
      }
    }

    private bool _removeLabels;

    public bool RemoveLabels
    {
      get { return _removeLabels; }
      set
      {
        if (_removeLabels != value)
        {
          _removeLabels = value;
          RaisePropertyChanged(() => RemoveLabels);
        }
      }
    }

    private string _texter = string.Empty;

    public string Texter
    {
      get { return _texter; }
      set
      {
        if (_texter != value)
        {
          _texter = value;
          RaisePropertyChanged(() => Texter);
        }
      }
    }

    private string _arrangedBy = string.Empty;

    public string ArrangedBy
    {
      get { return _arrangedBy; }
      set
      {
        if (_arrangedBy != value)
        {
          _arrangedBy = value;
          RaisePropertyChanged(() => ArrangedBy);
        }
      }
    }

    #endregion Properties

    #region    Special Accessors

    private void SetArrangedBy(string arrangedByText)
    {
      ArrangedBy = arrangedByText.Replace(ArrangedByPrefix, "");
    }

    public string GetArrangedBy()
    {
      return ArrangedByPrefix + ArrangedBy.Trim();
    }

    private void SetComposerAndTexter(string composerText)
    {
      if (!string.IsNullOrEmpty(composerText) && composerText.Contains(ComposerPrefix))
      {
        var ctex = composerText.Split(',');
        Composer = ctex[0].Replace(ComposerPrefix, "").Trim();
        Texter = ctex[1].Replace(TexterPrefix, "").Trim();
      }
      else
      {
        Composer = composerText;
        Texter = "";
      }
    }

    public string GetComposerAndTexter()
    {
      if (!string.IsNullOrEmpty(Texter))
      {
        return ComposerPrefix + Composer.Trim() + ", " + TexterPrefix + Texter.Trim();
      }

      return Composer.Trim();
    }

    private void SetSubTitleAndPattern(string subTitleText)
    {
      if (!string.IsNullOrEmpty(subTitleText) && subTitleText.Contains(PatternPrefix))
      {
        int patternIdx = subTitleText.IndexOf(PatternPrefix);
        SubTitle = subTitleText.Substring(0, patternIdx).Trim();
        Pattern = int.Parse(subTitleText.Substring(patternIdx + PatternPrefix.Length).Trim(' ', ')'));
      }
      else
      {
        SubTitle = subTitleText;
        Pattern = 0;
      }
    }

    public string GetSubTitleAndPattern()
    {
      if (Pattern != 0)
      {
        return (SubTitle.Trim() + PatternPrefix + Pattern + ")").Trim();
      }

      return SubTitle.Trim();
    }

    #endregion Special Accessors

    #region    Load & Save

    private void LoadFromXml()
    {
      var xdoc = new XmlDocument();
      xdoc.Load(this._fileName);
      foreach (XmlElement creditNode in xdoc.DocumentElement.SelectNodes("//credit-words").Cast<XmlElement>().ToList())
      {
        try
        {
          // remove all existing credit nodes
          string identifier = creditNode.Attributes["font-size"].Value + creditNode.Attributes["justify"].Value +
                              creditNode.Attributes["valign"].Value;

          switch (identifier)
          {
            case "24centertop":
              // title
              Title = creditNode.InnerText;
              break;
            case "14centertop":
              // subtitle
              SetSubTitleAndPattern(creditNode.InnerText);
              break;
            case "12righttop":
              // composer
              SetComposerAndTexter(creditNode.InnerText);
              break;
            case "10righttop":
              // arranged by
              SetArrangedBy(creditNode.InnerText);
              break;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("Header element not defined! (" + ex + ")");
        }
      }
    }

    public void Save()
    {
      var xmlFileName = new FileInfo(_fileName);
      if (!xmlFileName.Exists || xmlFileName.Extension != ".xml")
      {
        return;

      }
      var outputDirectory = Settings.Default.OutputPath.Trim('\\') + "\\";
      if (!Directory.Exists(outputDirectory))
      {
        Directory.CreateDirectory(outputDirectory);
      }

      var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      var tempDir = Directory.CreateDirectory(dir + "\\temp\\" + Guid.NewGuid());
      try
      {
        var tempXml = tempDir.FullName + "\\" + xmlFileName.Name;
        File.Copy(xmlFileName.FullName, tempXml);

        var xdoc = new XmlDocument();
        xdoc.Load(tempXml);
        foreach (XmlElement creditNode in xdoc.SelectNodes("//credit").Cast<XmlElement>().ToList())
        {
          // remove all existing credit nodes
          creditNode.ParentNode.RemoveChild(creditNode);
        }

        AppendCreditNode(xdoc.DocumentElement, "595.238", "1626.98", "24", "center", "top", Title.Trim());
        AppendCreditNode(xdoc.DocumentElement, "595.238", "1570.29", "14", "center", "top", GetSubTitleAndPattern());
        AppendCreditNode(xdoc.DocumentElement, "1133.79", "1559.98", "12", "right", "top", GetComposerAndTexter());
        AppendCreditNode(xdoc.DocumentElement, "1133.79", "1583.98", "10", "right", "top", GetArrangedBy());
        xdoc.Save(tempXml);

        var museScore = new MuseScoreApp();
        var museScoreFile = museScore.ConvertXMLtoMuseScore(tempXml);

        if (museScoreFile != null && File.Exists(museScoreFile))
        {
          ProcessMuseScoreFile(museScoreFile);
          var compressedMuseScoreFile = museScore.ConvertMuseScoretoCompressedMuseScore(museScoreFile);

          var targetMuseScoreFile = GetFileName(outputDirectory);

          File.Copy(compressedMuseScoreFile, targetMuseScoreFile, true);

          if (ExportPNG)
          {
            museScore.ConvertMuseScoreToPNG(compressedMuseScoreFile, targetMuseScoreFile.Replace(".mscz", ".png"), 96);
          }

          if (ExportPDF)
          {
            museScore.ConvertMuseScoreToPDF(compressedMuseScoreFile, targetMuseScoreFile.Replace(".mscz", ".pdf"));
          }

          Process.Start("file://" + Path.GetDirectoryName(targetMuseScoreFile));
        }
      }
      catch (Exception ex)
      {
        throw new HandledErrorException("Fehler!",
                                        "Beim Speichern ist ein Fehler aufgetreten." + Environment.NewLine +
                                        Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
      }
      finally
      {
        tempDir.Delete(true);
      }
    }

    private string GetFileName(string outputDirectory)
    {
      var songDir = outputDirectory + Title + "\\";
      if (!Directory.Exists(songDir))
      {
        Directory.CreateDirectory(songDir);
      }
      return songDir + Title.Trim().Replace(" ", "_") + (Pattern == 0 ? "" : FileNamePostfix + Pattern) + ".mscz";
    }

    private void ProcessMuseScoreFile(string museScoreFile)
    {
      try
      {
        var museScoreXmlFile = new XmlDocument();
        museScoreXmlFile.Load(museScoreFile);
        foreach (var museScoreTransformer in Transformers)
        {
          museScoreTransformer.ApplyTransformation(this, museScoreXmlFile);
        }
        museScoreXmlFile.Save(museScoreFile);
      }
      catch (Exception ex)
      {
        throw new HandledErrorException("Fehler!",
                                        "Beim Konvertieren ist ein Fehler aufgetreten." + Environment.NewLine +
                                        Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
      }
    }

    private void AppendCreditNode(XmlElement root, string defaultX, string defaultY, string fontSize, string justify, string valign, string content)
    {
      if (string.IsNullOrEmpty(content))
      {
        // do not add empty blocks
        return;
      }

      var doc = root.OwnerDocument;
      var creditNode = doc.CreateElement("credit");
      creditNode.AddAttribute("page", "1");
      var creditWordsNode = doc.CreateElement("credit-words");
      creditNode.AppendChild(creditWordsNode);
      creditWordsNode.AddAttribute("default-x", defaultX);
      creditWordsNode.AddAttribute("default-y", defaultY);
      creditWordsNode.AddAttribute("font-size", fontSize);
      creditWordsNode.AddAttribute("justify", justify);
      creditWordsNode.AddAttribute("valign", valign);
      creditWordsNode.InnerText = content;

      var defaultsNode = root.SelectSingleNode("defaults");
      if (defaultsNode != null)
      {
        root.InsertAfter(creditNode, defaultsNode);
      }
      else
      {
        root.AppendChild(creditNode);
      }
    }

    #endregion Load & Save
  }
}