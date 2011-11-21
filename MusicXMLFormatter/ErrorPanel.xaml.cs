using System.Windows;
using System.Windows.Controls;
using MusicXMLFormatter.Core;

namespace MusicXMLFormatter
{
  /// <summary>
  /// CustomControl to display an overlay error
  /// </summary>
  public class ErrorPanel : Control
  {
    public static readonly DependencyProperty HandledErrorExceptionProperty =
      DependencyProperty.Register("HandledErrorException", typeof (HandledErrorException), typeof (ErrorPanel), new PropertyMetadata(default(HandledErrorException), ExceptionChangedHandler));

    private static void ExceptionChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var errorPanel = d as ErrorPanel;
      if (errorPanel == null)
      {
        return;
      }

      errorPanel.Visibility = errorPanel.HandledErrorException != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public HandledErrorException HandledErrorException
    {
      get { return (HandledErrorException) GetValue(HandledErrorExceptionProperty); }
      set { SetValue(HandledErrorExceptionProperty, value); }
    }

    public ErrorPanel()
    {
      Visibility = Visibility.Collapsed;
    }

    protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      HandledErrorException = null;
    }

    static ErrorPanel()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorPanel), new FrameworkPropertyMetadata(typeof(ErrorPanel)));
    }
  }
}
