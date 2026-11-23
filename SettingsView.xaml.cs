using HomeMaintenanceMAUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeMaintenanceMAUI {
  /// <summary>Shows appearance controls and the Windows-startup setting.</summary>
  public partial class SettingsView : ContentView {
    /// <summary>Prevents a displayed preference refresh from saving it again.</summary>
    private bool _isShowingTheme;
    private IStartupAppService? _startupAppService;

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
      ShowStartupSetting();
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

    private void ShowStartupSetting() {
      _startupAppService = Application.Current?.Handler?.MauiContext?.Services
        .GetService<IStartupAppService>();

      if (_startupAppService is null) {
        WindowsStartupButton.IsEnabled = false;
        WindowsStartupStatusLabel.Text = "Windows startup is unavailable on this platform.";
        return;
      }

      WindowsStartupButton.IsEnabled = _startupAppService is not UnsupportedStartupAppService;
      WindowsStartupButton.Text = _startupAppService.IsEnabled
        ? "Disable Windows startup"
        : "Allow Windows startup";
      WindowsStartupStatusLabel.Text = _startupAppService is UnsupportedStartupAppService
        ? "Windows startup is unavailable on this platform."
        : _startupAppService.IsEnabled
          ? "The app will open when you sign in to Windows."
          : "Windows startup is disabled.";
    }

    private void OnWindowsStartupClicked(object? sender, EventArgs e) {
      if (_startupAppService is null) {
        ShowStartupSetting();
        return;
      }

      bool enabled = !_startupAppService.IsEnabled;
      if (!_startupAppService.SetEnabled(enabled)) {
        WindowsStartupStatusLabel.Text = "Windows startup could not be updated.";
        return;
      }

      ShowStartupSetting();
    }
  }
}
