using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MusicXMLFormatter.Properties;

namespace MusicXMLFormatter
{
  public class MuseScoreApp
  {
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

          
        }

        return _museScoreExe;
      }
    }

    private string MuseScorePath
    {
      get { return Path.GetDirectoryName(MuseScoreExe); }
      
    }

    public ImageSource ConvertMuseScoreToPNG(string museScoreFile, int dpi = 300)
    {
      var museScoreFileInfo = new FileInfo(museScoreFile);
      if (!museScoreFileInfo.Exists)
      {
        return null;
      }
      string imageFileName = museScoreFileInfo.FullName.Replace(".mscz", ".png ");
      ProcessStartInfo museScoreStartInfo = new ProcessStartInfo(MuseScoreExe)
                                                {
                                                  CreateNoWindow = true,
                                                  Arguments = "-r " + dpi + " -o " + imageFileName + " " + museScoreFileInfo.FullName,
                                                  WorkingDirectory = MuseScorePath,
                                                  UseShellExecute = true
                                                };
      var process = Process.Start(museScoreStartInfo);
      process.WaitForExit(10000);

      if (File.Exists(imageFileName))
      {
        return new BitmapImage(new Uri("file://" + imageFileName));
      }

      return null;
    }

    public string ConvertXMLtoMuseScore(string xmlFile)
    {
      var musicXmlFileInfo = new FileInfo(xmlFile);
      if (!musicXmlFileInfo.Exists || musicXmlFileInfo.Extension != ".xml")
      {
        return null;
      }
      string museScoreFileName = musicXmlFileInfo.FullName.Replace(".xml", ".mscx");

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

      return null;
    }

    public string ConvertMuseScoretoCompressedMuseScore(string mscxFile)
    {
      var museScoreFileInfo = new FileInfo(mscxFile);
      if (!museScoreFileInfo.Exists || museScoreFileInfo.Extension != ".mscx")
      {
        return null;
      }
      string compressedMuseScoreFileName = museScoreFileInfo.FullName.Replace(".mscx", ".mscz");

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

      return null;
    }
  }
}