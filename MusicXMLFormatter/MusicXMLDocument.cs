using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Practices.Prism.ViewModel;

namespace MusicXMLFormatter
{
    public class MusicXMLDocument : NotificationObject
    {
        private readonly string _fileName;

        public MusicXMLDocument(string fileName)
        {
            this._fileName = fileName;
            this.LoadFromXml();
        }

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
                            SubTitle = creditNode.InnerText;
                            break;
                        case "12righttop":
                            // composer
                            Composer = creditNode.InnerText;
                            break;
                        case "12lefttop":
                            // texter
                            Texter = creditNode.InnerText;
                            break;
                        case "10righttop":
                            // arranged by
                            ArrangedBy = creditNode.InnerText.Replace("arr. ", "");
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
                AppendCreditNode(xdoc.DocumentElement, "595.238", "1570.29", "14", "center", "top", SubTitle.Trim());
                AppendCreditNode(xdoc.DocumentElement, "1133.79", "1559.98", "12", "right", "top", Composer.Trim());
                AppendCreditNode(xdoc.DocumentElement, "1133.79", "1583.98", "10", "right", "top", "arr. " + ArrangedBy.Trim());
                AppendCreditNode(xdoc.DocumentElement, "56.6893", "1559.98", "12", "left", "top", Texter.Trim());
                xdoc.Save(tempXml);

                var museScore = new MuseScoreApp();
                var museScoreFile = museScore.ConvertXMLtoMuseScore(tempXml);

                if (museScoreFile != null && File.Exists(museScoreFile))
                {
                    ProcessMuseScoreFile(museScoreFile);
                    var compressedMuseScoreFile = museScore.ConvertMuseScoretoCompressedMuseScore(museScoreFile);
                    var targetMuseScoreFile = xmlFileName.FullName.Replace(".xml", ".mscz");

                    File.Copy(compressedMuseScoreFile, targetMuseScoreFile, true);

                    Process.Start("file://" + Path.GetDirectoryName(targetMuseScoreFile));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problems! " + ex);
            }
            finally
            {
                tempDir.Delete(true);
            }
        }

        private void ProcessMuseScoreFile(string museScoreFile)
        {


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
    }
}