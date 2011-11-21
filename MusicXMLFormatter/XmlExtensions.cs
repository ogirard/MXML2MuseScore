using System.Xml;

namespace MusicXMLFormatter
{
    public static class XmlExtensions
    {
         public static void AddAttribute(this XmlElement element, string name, string value)
         {
             var attr = element.OwnerDocument.CreateAttribute(name);
             attr.Value = value;
             element.Attributes.Append(attr);
         }
    }
}