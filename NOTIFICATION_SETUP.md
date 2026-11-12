# HomeMaintenanceMAUI — Windows notification integration

This is a minimal source-code patch for the user's .NET 9 MAUI project. The original
project was unpackaged (`WindowsPackageType=None`) so `Package.appxmanifest` is
intentionally unchanged; `AppNotificationManager.Register()` handles registration.

## How to run

1. Open `HomeMaintenanceMAUI.sln` in Visual Studio on Windows.
2. Restore NuGet packages and select the Windows Machine target.
3. Start WITHOUT administrator privileges.
4. On the login page, click **Test Windows Notification**. A toast should appear
   and be listed in Notification Center (subject to Windows notifications settings).
5. Log in normally to test the existing login path.

## Reminder behavior

After a successful login, the app reads the logged-in account's saved `TaskRecord`
entries and sends one summary notification when at least one pending, unarchived
task is due today through 7 days from today (inclusive). Overdue/completed/archived
tasks do not trigger the summary. Since the current app has no task-creation UI,
there may be no saved tasks to trigger a real reminder yet.

## Files changed

- `Platforms/Windows/App.xaml.cs`: register notifications on startup.
- `Platforms/Windows/WindowsNotificationService.cs`: Windows toast sender (new).
- `MainPage.xaml`: add a temporary notification test button.
- `MainPage.xaml.cs`: test button and login-time reminder check.
- `Services/AccountStore.cs`: read tasks from the existing account JSON.
- `Services/MaintenanceReminderService.cs`: filter pending, unarchived tasks due in 7 days (new).
- `DashboardPage.xaml` / `DashboardPage.xaml.cs`: make dashboard a `ContentPage`
  so it is a valid Shell route instead of a `ContentView`.

## Notes

- .NET MAUI normally brings Windows App SDK transitively. If the AppNotifications
  namespace isn't available, check the installed compatible Windows App SDK
  dependency. Do not indiscriminately add an incompatible package version.
- This integration has not been compiled or executed here because the runtime
  is not Windows / Visual Studio. Verify it locally in Visual Studio.
- Existing app account records and account storage layout were not modified.
- Recurrence generation and deduplication are future work.
- This does not deliver a notification when the app is not running.
- If notifications are not visible, check Windows Settings > System > Notifications
  and ensure Do Not Disturb is not suppressing banners.
