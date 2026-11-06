namespace HomeMaintenanceMAUI {
  /// <summary>
  /// Keeps the task-filter picker readable against Workshop's paper background in any app theme.
  /// </summary>
  public class WorkshopFilterPicker : Picker {
    /// <summary>
    /// Gives the Windows combo box a dark native arrow and readable light-theme menu.
    /// </summary>
    protected override void OnHandlerChanged() {
      base.OnHandlerChanged();
#if WINDOWS
      if (Handler?.PlatformView is Microsoft.UI.Xaml.Controls.ComboBox picker) {
        // Scope the contrast fix to this native control without changing the app's theme.
        picker.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
      }
#endif
    }
  }
}
