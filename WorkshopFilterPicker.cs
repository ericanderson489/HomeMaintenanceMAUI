namespace HomeMaintenanceMAUI {
  /// <summary>
  /// Keeps Workshop dropdown arrows and menus consistent with the selected appearance.
  /// </summary>
  public class WorkshopFilterPicker : Picker {
    /// <summary>
    /// Initializes native appearance and observes future changes without retaining old pickers.
    /// </summary>
    protected override void OnHandlerChanged() {
      base.OnHandlerChanged();
      // BEFORE: OnHandlerChanged only called base and forced the Windows picker to Light.
      // AFTER: Remove any previous subscription, then initialize and observe our weak event
      // while a native handler exists. The native change below now supports Dark and Light.
      WorkshopTheme.ThemeChanged -= OnThemeChanged;
      if (Handler is not null) {
        WorkshopTheme.Initialize();
        WorkshopTheme.ThemeChanged += OnThemeChanged;
        ApplyTheme();
      }
    }

    /// <summary>Refreshes an existing dropdown after the Settings switch is toggled.</summary>
    private void OnThemeChanged(object? sender, EventArgs e) {
      // BEFORE: No live theme callback existed. AFTER: Reapply this control's native theme.
      ApplyTheme();
    }

    /// <summary>Changes only the native theme, preserving items, titles, and selected values.</summary>
    private void ApplyTheme() {
#if WINDOWS
      if (Handler?.PlatformView is Microsoft.UI.Xaml.Controls.ComboBox picker) {
        // BEFORE: picker.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
        // AFTER: Match the saved appearance instead of retaining a light menu in dark mode.
        picker.RequestedTheme = WorkshopTheme.IsDarkMode
            ? Microsoft.UI.Xaml.ElementTheme.Dark
            : Microsoft.UI.Xaml.ElementTheme.Light;
      }
#endif
    }
  }
}
