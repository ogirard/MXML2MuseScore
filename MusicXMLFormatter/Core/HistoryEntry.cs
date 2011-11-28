using System.Xml.Linq;

namespace MusicXMLFormatter.Core
{
  public class HistoryEntry
  {
    public HistoryEntry(XElement entry)
    {
      Title = entry.Element("title").Value;
      SubTitle = entry.Element("subtitle").Value;
      Composer = entry.Element("composer").Value;
      Pattern = int.Parse(entry.Element("pattern").Value);
      ExportPNG = bool.Parse(entry.Element("exportpng").Value);
      ExportPDF = bool.Parse(entry.Element("exportpdf").Value);
      ExtractVoice = bool.Parse(entry.Element("extractvoice").Value);
      RemoveLabels = bool.Parse(entry.Element("removelabels").Value);
      Texter = entry.Element("texter").Value;
      ArrangedBy = entry.Element("arrangedby").Value;
    }

    public HistoryEntry(ScoreDocument doc)
    {
      Title = doc.Title.Trim();
      SubTitle = doc.SubTitle.Trim();
      Composer = doc.Composer.Trim();
      Pattern = doc.Pattern;
      ExportPNG = doc.ExportPNG;
      ExportPDF = doc.ExportPDF;
      ExtractVoice = doc.ExtractVoice;
      RemoveLabels = doc.RemoveLabels;
      Texter = doc.Texter.Trim();
      ArrangedBy = doc.ArrangedBy.Trim();
    }

    public XElement ToXmlNode()
    {
      var entry = new XElement("entry");
      entry.Add(new XElement("title", Title));
      entry.Add(new XElement("subtitle", SubTitle));
      entry.Add(new XElement("composer", Composer));
      entry.Add(new XElement("pattern", Pattern.ToString()));
      entry.Add(new XElement("exportpng", ExportPNG.ToString()));
      entry.Add(new XElement("exportpdf", ExportPDF.ToString()));
      entry.Add(new XElement("extractvoice", ExtractVoice.ToString()));
      entry.Add(new XElement("removelabels", RemoveLabels.ToString()));
      entry.Add(new XElement("texter", Texter));
      entry.Add(new XElement("arrangedby", ArrangedBy));

      return entry;
    }

    public void ApplyScoreDocument(ScoreDocument doc)
    {
      doc.Title = Title;
      doc.SubTitle = SubTitle;
      doc.Composer = Composer;
      doc.Pattern = Pattern;
      doc.ExportPNG = ExportPNG;
      doc.ExportPDF = ExportPDF;
      doc.ExtractVoice = ExtractVoice;
      doc.RemoveLabels = RemoveLabels;
      doc.Texter = Texter;
      doc.ArrangedBy = ArrangedBy;
    }

    #region    Properties

    public string Title { get; set; }

    public string SubTitle { get; set; }

    public string Composer { get; set; }

    public int Pattern { get; set; }

    public bool ExportPNG { get; set; }

    public bool ExportPDF { get; set; }

    public bool ExtractVoice { get; set; }

    public bool RemoveLabels { get; set; }

    public string Texter { get; set; }

    public string ArrangedBy { get; set; }

    #endregion Properties

    public override string ToString()
    {
      return this.Title;
    }
  }
}