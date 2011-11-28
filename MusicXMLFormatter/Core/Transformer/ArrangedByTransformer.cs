using System.Xml;
using MusicXMLFormatter.Core;

namespace MusicXMLFormatter.Transformer
{
  public class ArrangedByTransformer : IMuseScoreTransformer
  {
    public void ApplyTransformation(ScoreDocument score, XmlDocument museScoreXmlFile)
    {
      var composerNode = museScoreXmlFile.SelectSingleNode("//Text[subtype[text()='Composer']]/html-data//p") as XmlElement;
      if (composerNode == null)
      {
        return;
      }

      composerNode.InnerXml = "<span style=\"font-size:12pt;\">" + score.GetComposerAndTexter() + "</span><br/>" +
                              "<span style=\"font-size:9pt;\">" + score.GetArrangedBy() + "</span>";
    }
  }
}