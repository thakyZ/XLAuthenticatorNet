using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Windows;

/// <summary>
/// The update loading window class
/// </summary>
/// <seealso cref="Window"/>
public partial class UpdateLoadingWindow : Window  {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private UpdateLoadingWindowViewModel Model => (this.DataContext as UpdateLoadingWindowViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdateLoadingWindow"/> class
  /// </summary>
  internal UpdateLoadingWindow() {
    InitializeComponent();
    this.DataContext = new UpdateLoadingWindowViewModel(this);
    MouseMove += UpdateLoadingWindow_OnMouseMove;
  }

  /// <summary>
  /// Updates the loading window on mouse move using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void UpdateLoadingWindow_OnMouseMove(object? sender, MouseEventArgs e) {
    if (e.LeftButton == MouseButtonState.Pressed) {
      this.DragMove();
    }
  }
}