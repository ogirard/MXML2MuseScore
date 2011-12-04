using System.Xml;

namespace MusicXMLFormatter.Core.Transformer
{
  public class RemoveCaptionsTransformer : IMuseScoreTransformer
  {
    public void ApplyTransformation(ScoreDocument score, XmlDocument museScoreXmlFile)
    {
      if (!score.RemoveLabels)
      {
        return;
      }

      var shortNameNodes = museScoreXmlFile.SelectNodes("//shortName[subtype[text()='InstrumentShort']]/html-data");
      if (shortNameNodes == null)
      {
        return;
      }

      var longNameNodes = museScoreXmlFile.SelectNodes("//name[subtype[text()='InstrumentLong']]/html-data");
      if (longNameNodes == null)
      {
        return;
      }

      foreach (var shortNameNode in shortNameNodes)
      {
        ((XmlElement)shortNameNode).InnerXml = "";
      }

      foreach (var longNameNode in longNameNodes)
      {
        ((XmlElement)longNameNode).InnerXml = "";
      }
    }
  }
}