using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using XLAuth.Models.ViewModel;

namespace XLAuth.Windows;

/// <summary>
/// The update loading window class
/// </summary>
/// <seealso cref="Window"/>
public partial class UpdateLoadingWindow : Window  {
  ///// <summary>
  ///// Gets the value of the model
  ///// </summary>
  //private UpdateLoadingWindowViewModel Model
  //  => (this.DataContext as UpdateLoadingWindowViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdateLoadingWindow"/> class
  /// </summary>
  internal UpdateLoadingWindow() {
    this.InitializeComponent();
    this.DataContext = new UpdateLoadingWindowViewModel(this);
    this.MouseMove += this.OnMouseMove;
  }

  /// <summary>
  /// Updates the loading window on mouse move using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="event">The </param>
  private void OnMouseMove(object? sender, MouseEventArgs @event) {
    if (@event.LeftButton == MouseButtonState.Pressed) {
      this.DragMove();
    }
  }
}
