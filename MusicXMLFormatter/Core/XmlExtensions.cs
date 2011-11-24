using System.Xml;

namespace MusicXMLFormatter.Core
{
  public static class XmlExtensions
  {
    public static void AddAttribute(this XmlElement element, string name, string value)
    {
      var attr = element.OwnerDocument.CreateAttribute(name);
      attr.Value = value;
      element.Attributes.Append(attr);
    }

    public static XmlElement AddChildNode(this XmlElement element, string name, string value = null)
    {
      var elem = element.OwnerDocument.CreateElement(name);
      if (value != null)
      {
        elem.InnerText = value;
      }
      element.AppendChild(elem);
      return elem;
    }
  }
}