# Windows login reminders — installation and behavior

This update builds on the previous Windows notifications integration.

## Copy into your existing MAUI project

1. Switch to the branch where your previous notification files were installed, and save/commit any outstanding changes.
2. Close Visual Studio.
3. Copy the three files below to the same relative paths in your project, replacing the existing files:
   - `MainPage.xaml`
   - `MainPage.xaml.cs`
   - `Services/MaintenanceReminderService.cs`
4. Reopen the solution, choose **Windows Machine**, and rebuild.

There are no project (`.csproj`) or manifest changes in this update. Keep `Platforms/Windows/App.xaml.cs`, `Platforms/Windows/WindowsNotificationService.cs`, and `Services/AccountStore.cs` from the previous integration.

## What happens on a successful login

- After login and dashboard navigation, one Windows notification is created **if** any eligible task exists.
- **Overdue**: a pending, unarchived task with a due date *before today*.
- **Due soon**: a pending, unarchived task due *today through seven days from today*, inclusive.
- Both categories appear in the same notification when both have matching tasks. Each shows a count and up to two task names/dates, followed by a remaining-task count if necessary.
- Completed and archived tasks are excluded. If no tasks match, no notification is shown.
- The test-notification button and its click handler have been removed.
- The login welcome dialog remains; the reminder is sent after it is dismissed.

**Current limitation:** The provided MAUI app still has no task-creation screen. A newly created account has zero saved tasks, so signing in will not show a reminder until tasks are stored by another app feature or development fixture. The notification does not run while the app is closed.

**Verification:** Source/XML checks pass, but this environment lacks Windows/Visual Studio and cannot execute the MAUI Windows target. Verify it in Visual Studio on Windows with notification permissions enabled.
