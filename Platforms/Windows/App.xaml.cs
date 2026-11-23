using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
using Microsoft.Windows.AppNotifications;

namespace HomeMaintenanceMAUI.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
    bool startup = true;
    this.InitializeComponent();
    if (startup) {
      RegisterForWindowsStartup();
    } else {
      UnregisterFromWindowsStartup();
    }
  }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Register the callback before registering with Windows.
        // This project is unpackaged (WindowsPackageType=None): no manifest
        // COM extension or manually defined application ID is needed.
        var manager = AppNotificationManager.Default;
        manager.NotificationInvoked += OnNotificationInvoked;
        manager.Register();

        base.OnLaunched(args);
    }

    private static void OnNotificationInvoked(
        AppNotificationManager sender,
        AppNotificationActivatedEventArgs args)
    {
        // The callback may run on a background thread. If you add UI
        // navigation later, dispatch it to the UI thread first.
        System.Diagnostics.Debug.WriteLine(
            $"Windows notification clicked: {args.Argument}");
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

  private static void RegisterForWindowsStartup() {
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

  private static void UnregisterFromWindowsStartup() {
    try {
      using RegistryKey? startupKey = Registry.CurrentUser.OpenSubKey(
          @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);

      startupKey?.DeleteValue("HomeMaintenanceMAUI", throwOnMissingValue: false);
    } catch (Exception) {
      // Startup registration should not prevent the app from opening.
    }
  }
}


