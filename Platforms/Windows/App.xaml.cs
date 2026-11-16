using Microsoft.UI.Xaml;
using Microsoft.Win32;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HomeMaintenanceMAUI.WinUI {
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public partial class App : MauiWinUIApplication {
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
      this.InitializeComponent();
      RegisterForWindowsStartup();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private static void RegisterForWindowsStartup() {
      bool startup = false;
      if (startup) {
        try {
          string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;

          if (string.IsNullOrWhiteSpace(executablePath)) {
            return;
          }

          using RegistryKey? startupKey = Registry.CurrentUser.CreateSubKey(
              @"Software\Microsoft\Windows\CurrentVersion\Run");

          startupKey?.SetValue("HomeMaintenanceMAUI", $"\"{executablePath}\"");
        } catch (Exception) {
          // Startup registration should not prevent the app from opening.
        }
      }
    }
  }

}
