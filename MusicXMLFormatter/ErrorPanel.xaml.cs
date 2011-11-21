using System.Windows;
using System.Windows.Controls;

namespace MusicXMLFormatter
{
  /// <summary>
  /// CustomControl to display an overlay error
  /// </summary>
  public class ErrorPanel : Control
  {
    static ErrorPanel()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorPanel), new FrameworkPropertyMetadata(typeof(ErrorPanel)));
    }
  }
}
