namespace MusicXMLFormatter.Core
{
  public interface IMuseScoreTransformer
  {
    /// <summary>
    /// Transformation to be applied on (uncompressed) muse score file
    /// </summary>
    /// <param name="score">Score document with settings for transformation</param>
    /// <param name="museScoreFile"></param>
    void ApplyTransformation(ScoreDocument score, string museScoreFile);
  }
}