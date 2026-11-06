namespace HomeMaintenanceMAUI {
  /// <summary>
  /// Preserves native button behavior with the Workshop sidebar's left-aligned text.
  /// </summary>
  public class WorkshopNavigationButton : Button {
    /// <summary>Aligns the Windows label when MAUI creates its native button.</summary>
    protected override void OnHandlerChanged() {
      base.OnHandlerChanged();
#if WINDOWS
      if (Handler?.PlatformView is Microsoft.UI.Xaml.Controls.Button button) {
        button.HorizontalContentAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left;
      }
#endif
    }
  }
}
