namespace HomeMaintenanceMAUI {
  // BEFORE: No SettingsView existed.
  // AFTER: This new GUI view changes only our appearance preference. The Windows startup
  // button remains a disabled handoff for the team; no startup registration is performed.
  /// <summary>Shows appearance controls and the future Windows-startup setting.</summary>
  public partial class SettingsView : ContentView {
    /// <summary>Prevents a displayed preference refresh from saving it again.</summary>
    private bool _isShowingTheme;

    /// <summary>Loads the settings controls and observes the theme while the view is loaded.</summary>
    public SettingsView() {
      InitializeComponent();
      Loaded += OnSettingsLoaded;
      Unloaded += OnSettingsUnloaded;
    }

    /// <summary>Displays the saved theme and starts observing appearance changes.</summary>
    private void OnSettingsLoaded(object? sender, EventArgs e) {
      WorkshopTheme.Initialize();
      WorkshopTheme.ThemeChanged -= OnThemeChanged;
      WorkshopTheme.ThemeChanged += OnThemeChanged;
      ShowTheme();
    }

    /// <summary>Releases the preference observer when the settings view leaves the window.</summary>
    private void OnSettingsUnloaded(object? sender, EventArgs e) {
      WorkshopTheme.ThemeChanged -= OnThemeChanged;
    }

    /// <summary>Keeps the switch synchronized when another GUI surface changes appearance.</summary>
    private void OnThemeChanged(object? sender, EventArgs e) {
      ShowTheme();
    }

    /// <summary>Updates the switch without treating a theme refresh as a user request.</summary>
    private void ShowTheme() {
      _isShowingTheme = true;
      try {
        DarkModeSwitch.IsToggled = WorkshopTheme.IsDarkMode;
      } finally {
        _isShowingTheme = false;
      }
    }

    /// <summary>Applies and saves only the explicitly selected appearance preference.</summary>
    private void OnDarkModeToggled(object? sender, ToggledEventArgs e) {
      if (!_isShowingTheme) {
        WorkshopTheme.SetDarkMode(e.Value);
      }
    }
  }
}
