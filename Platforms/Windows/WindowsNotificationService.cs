using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace HomeMaintenanceMAUI.Platforms.Windows;

// A Windows-only helper for sending notifications through Notification Center.
public static class WindowsNotificationService
{
    public static void SendNotification(string title, string message)
    {
        var notification = new AppNotificationBuilder()
            .AddText(title)
            .AddText(message)
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }
}
