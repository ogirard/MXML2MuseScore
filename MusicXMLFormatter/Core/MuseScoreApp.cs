using System;
using System.Diagnostics;
using System.IO;
using MusicXMLFormatter.Properties;

namespace MusicXMLFormatter.Core
{
  public class MuseScoreApp
  {
    public const string MuseScoreFileExt = ".mscx";
    public const string CompressedMuseScoreFileExt = ".mscz";
    public const string PNGFileExt = ".png";
    public const string PDFFileExt = ".pdf";
    public const string XMLFileExt = ".xml";

    private string _museScoreExe;

    private string MuseScoreExe
    {
      get
      {
        if (_museScoreExe == null)
        {
          string museScoreExe = Settings.Default.MuseScoreExe.Replace("${MuseScoreExeDefault}", Settings.Default.MuseScoreExeDefault);
          string museScoreExe86 = museScoreExe.Replace("${ProgramFiles}", Environment.GetEnvironmentVariable("ProgramFiles(x86)"));

          if (File.Exists(museScoreExe86))
          {
            _museScoreExe = museScoreExe86;
            return _museScoreExe;
          }

          string museScoreExe64 = museScoreExe.Replace("${ProgramFiles}", Environment.GetEnvironmentVariable("ProgramFiles"));

          if (File.Exists(museScoreExe64))
          {
            _museScoreExe = museScoreExe64;
            return _museScoreExe;
          }

          throw new HandledErrorException("MuseScore nicht gefunden!",
                                          "Unter " + Settings.Default.MuseScoreUrl +
                                          " k�nnen Sie MuseScore herunterladen.");
        }

        return _museScoreExe;
      }
    }

    private string MuseScorePath
    {
      get { return Path.GetDirectoryName(MuseScoreExe); }

    }

    public void ConvertMuseScoreToPNG(string museScoreFile, string imageFileName, int dpi = 300)
    {
      var museScoreFileInfo = new FileInfo(museScoreFile);
      if (!museScoreFileInfo.Exists)
      {
        throw new HandledErrorException("Datei nicht gefunden!",
                                        "Die MuseScore Datei '" + museScoreFile + "' konnte nicht gefunden werden.");
      }

      ProcessStartInfo museScoreStartInfo = new ProcessStartInfo(MuseScoreExe)
                                                {
                                                  CreateNoWindow = true,
                                                  Arguments = "-r " + dpi + " -o \"" + imageFileName + "\" \"" + museScoreFileInfo.FullName + "\"",
                                                  WorkingDirectory = MuseScorePath
                                                };
      var process = Process.Start(museScoreStartInfo);
      process.WaitForExit(10000);

      if (!File.Exists(imageFileName.Replace(PNGFileExt, "-1" + PNGFileExt)))
      {
        throw new HandledErrorException("Fehler!",
                                "Aus der MuseScore Datei '" + museScoreFile +
                                "' konnte kein PNG Bild generiert werden.");
      }
    }

    public void ConvertMuseScoreToPDF(string museScoreFile, string pdfFileName)
    {
      var museScoreFileInfo = new FileInfo(museScoreFile);
      if (!museScoreFileInfo.Exists)
      {
        throw new HandledErrorException("Datei nicht gefunden!",
                                        "Die MuseScore Datei '" + museScoreFile + "' konnte nicht gefunden werden.");
      }

      ProcessStartInfo museScoreStartInfo = new ProcessStartInfo(MuseScoreExe)
      {
        CreateNoWindow = true,
        Arguments = " -o \"" + pdfFileName + "\" \"" + museScoreFileInfo.FullName + "\"",
        WorkingDirectory = MuseScorePath
      };
      var process = Process.Start(museScoreStartInfo);
      process.WaitForExit(10000);

      if (!File.Exists(pdfFileName))
      {
        throw new HandledErrorException("Fehler!",
                                        "Aus der MuseScore Datei '" + museScoreFile +
                                        "' konnte keine PDF Datei generiert werden.");
      }
    }

    public string ConvertXMLtoMuseScore(string xmlFile)
    {
      var musicXmlFileInfo = new FileInfo(xmlFile);
      if (!musicXmlFileInfo.Exists || musicXmlFileInfo.Extension != XMLFileExt)
      {
        throw new HandledErrorException("Fehler!",
                                        "Die MusicXML Datei '" + xmlFile +
                                        "' konnte nicht gefunden werden.");
      }
      string museScoreFileName = musicXmlFileInfo.FullName.Replace(XMLFileExt, MuseScoreFileExt);

      if (File.Exists(museScoreFileName))
      {
        File.Delete(museScoreFileName);
      }

      ProcessStartInfo museScoreStartInfo = new ProcessStartInfo(MuseScoreExe)
      {
        CreateNoWindow = true,
        Arguments = "-o \"" + museScoreFileName + "\" \"" + xmlFile + "\"",
        WorkingDirectory = MuseScorePath
      };
      var process = Process.Start(museScoreStartInfo);
      process.WaitForExit(10000);

      if (File.Exists(museScoreFileName))
      {
        return museScoreFileName;
      }

      throw new HandledErrorException("Fehler!",
                                      "Die MusicXML Datei '" + xmlFile +
                                      "' konnte nicht in eine MuseScore Datei umgewandelt werden.");
    }

    public string ConvertMuseScoretoCompressedMuseScore(string mscxFile)
    {
      var museScoreFileInfo = new FileInfo(mscxFile);
      if (!museScoreFileInfo.Exists || museScoreFileInfo.Extension != MuseScoreFileExt)
      {
        throw new HandledErrorException("Fehler!",
                                "Die MuseScore Datei '" + mscxFile +
                                "' konnte nicht gefunden werden.");
      }
      string compressedMuseScoreFileName = museScoreFileInfo.FullName.Replace(MuseScoreFileExt, CompressedMuseScoreFileExt);

      if (File.Exists(compressedMuseScoreFileName))
      {
        File.Delete(compressedMuseScoreFileName);
      }

      ProcessStartInfo museScoreStartInfo = new ProcessStartInfo(MuseScoreExe)
      {
        CreateNoWindow = true,
        Arguments = "-o \"" + compressedMuseScoreFileName + "\" \"" + museScoreFileInfo.FullName + "\"",
        WorkingDirectory = MuseScorePath
      };
      var process = Process.Start(museScoreStartInfo);
      process.WaitForExit(10000);

      if (File.Exists(compressedMuseScoreFileName))
      {
        return compressedMuseScoreFileName;
      }

      throw new HandledErrorException("Fehler!",
                                "Die MuseScore Datei '" + mscxFile +
                                "' konnte nicht in eine komprimierte MuseScore Datei umgewandelt werden.");
    }
  }
}