using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace HomeMaintenanceMAUI {
  // BEFORE: No WorkshopTheme class, appearance preference, or dark palette existed.
  // AFTER: This GUI-only class saves the chosen appearance and updates our semantic colors.
  // The team's Colors.xaml, application startup, task records, and storage remain unchanged.
  /// <summary>Applies and remembers the Workshop's light or dark presentation.</summary>
  internal static class WorkshopTheme {
    /// <summary>The appearance-only preference key; independent of all account data.</summary>
    internal const string PreferenceKey = "Workshop.DarkMode";

    /// <summary>Weak subscriptions avoid keeping closed pages and pickers alive.</summary>
    private static readonly WeakEventManager ThemeEvents = new();

    /// <summary>Our semantic colors, with the original light palette and dark equivalents.</summary>
    private static readonly IReadOnlyDictionary<string, (string Light, string Dark)> Palette =
        new Dictionary<string, (string Light, string Dark)> {
          ["WorkshopText"] = ("#192D35", "#F2EFE6"),
          ["WorkshopSurface"] = ("#FFFCF5", "#1D2C34"),
          ["WorkshopCanvas"] = ("#F4F0E6", "#101C22"),
          ["WorkshopBorder"] = ("#B7C0BE", "#596D76"),
          ["WorkshopTableHeader"] = ("#B7C0BE", "#344B56"),
          ["WorkshopSecondaryText"] = ("#526771", "#B8C9D0"),
          ["WorkshopDanger"] = ("#B84432", "#FF9A84"),
          ["WorkshopSelection"] = ("#DFE9EC", "#355363"),
          ["WorkshopOnNavy"] = ("#FFFCF5", "#FFFCF5"),
          ["WorkshopOnGold"] = ("#192D35", "#192D35"),
          ["WorkshopNavigationSelected"] = ("#192D35", "#102A35"),
          ["WorkshopActionText"] = ("#B87800", "#F0BE45")
        };

    /// <summary>Whether the saved preference has been read for this application process.</summary>
    private static bool _initialized;

    /// <summary>The current appearance; light preserves the previous default.</summary>
    private static bool _isDarkMode;

    /// <summary>Notifies GUI controls after both palette and native appearance are updated.</summary>
    internal static event EventHandler ThemeChanged {
      add { ThemeEvents.AddEventHandler(value, nameof(ThemeChanged)); }
      remove { ThemeEvents.RemoveEventHandler(value, nameof(ThemeChanged)); }
    }

    /// <summary>Gets the user's saved or currently selected dark-mode preference.</summary>
    internal static bool IsDarkMode {
      get {
        Initialize();
        return _isDarkMode;
      }
    }

    /// <summary>Restores appearance when our first GUI page attaches, without editing startup.</summary>
    internal static void Initialize() {
      if (_initialized || Application.Current is null) {
        return;
      }

      _isDarkMode = Preferences.Default.Get(PreferenceKey, false);
      _initialized = true;
      ApplyPalette();
    }

    /// <summary>Persists a user selection and refreshes already open Workshop controls.</summary>
    /// <param name="enabled">True selects dark mode; false restores the original light mode.</param>
    internal static void SetDarkMode(bool enabled) {
      Initialize();
      if (Application.Current is null) {
        return;
      }

      // This preference changes appearance only; it does not enable Windows startup.
      Preferences.Default.Set(PreferenceKey, enabled);
      _isDarkMode = enabled;
      ApplyPalette();
      ThemeEvents.HandleEvent(typeof(WorkshopTheme), EventArgs.Empty, nameof(ThemeChanged));
    }

    /// <summary>Updates only our resource keys and the application's requested native theme.</summary>
    private static void ApplyPalette() {
      if (Application.Current is not Application app) {
        return;
      }

      foreach (var color in Palette) {
        app.Resources[color.Key] = Color.FromArgb(
            _isDarkMode ? color.Value.Dark : color.Value.Light);
      }
      app.UserAppTheme = _isDarkMode ? AppTheme.Dark : AppTheme.Light;
    }
  }
}
