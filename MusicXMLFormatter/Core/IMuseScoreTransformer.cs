using System.Xml;

namespace MusicXMLFormatter.Core
{
  public interface IMuseScoreTransformer
  {
    /// <summary>
    /// Transformation to be applied on (uncompressed) muse score file
    /// </summary>
    /// <param name="score">Score document with settings for transformation</param>
    /// <param name="museScoreXmlFile">Content of the uncompressed MuseScore file as <see cref="XmlDocument"/></param>
    void ApplyTransformation(ScoreDocument score, XmlDocument museScoreXmlFile);
  }
}