using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;

namespace HomeMaintenanceMAUI.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
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
}
