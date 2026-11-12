using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Services;

namespace HomeMaintenanceMAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateAccountPage));
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Get the username and password entered by the user.
            string username = UsernameEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";

            // Make sure both fields contain information.
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrEmpty(password))
            {
                await DisplayAlert(
                    "Missing Information",
                    "Please enter your username and password.",
                    "OK");

                return;
            }

            try
            {
                // Load accounts previously saved as JSON.
                AccountStore accountStore = new();
                List<Account> accounts = accountStore.LoadAccounts();

                // Find the account matching the entered username.
                Account? account = accounts.FirstOrDefault(a =>
                    string.Equals(
                        a.GetAccountUserName(),
                        username,
                        StringComparison.OrdinalIgnoreCase));

                // Check that the account exists and its password matches.
                if (account is null || !account.VerifyPassword(password))
                {
                    await DisplayAlert(
                        "Login Failed",
                        "Incorrect username or password.",
                        "OK");

                    return;
                }

                // Login succeeded.
                await DisplayAlert(
                    "Login Successful",
                    $"Welcome back, {account.GetAccountFirstName()}!",
                    "OK");

                // Capture active account to pass to other content pages
                UserSession.CurrentAccount = account;

                // Navigate to the dashboard on login.
                await Shell.Current.GoToAsync(nameof(DashboardPage));

                // Notify the signed-in user about pending, unarchived tasks:
                // overdue tasks (before today) and tasks due within 7 days.
#if WINDOWS
                try
                {
                    var tasks = accountStore.LoadTasks(account.GetAccountUserName());
                    string? reminder = MaintenanceReminderService.BuildLoginReminder(tasks);

                    if (reminder is not null)
                    {
                        Platforms.Windows.WindowsNotificationService.SendNotification(
                            "Maintenance reminders", reminder);
                    }
                }
                catch (Exception notificationError)
                {
                    // Notification problems must not undo a successful login.
                    System.Diagnostics.Debug.WriteLine(
                        $"Unable to display maintenance reminder: {notificationError}");
                }
#endif

            }
            catch (InvalidDataException)
            {
                await DisplayAlert(
                    "Account Data Error",
                    "One or more saved account files contain invalid information.",
                    "OK");
            }
            catch (IOException)
            {
                await DisplayAlert(
                    "Storage Error",
                    "Unable to read account information.",
                    "OK");
            }
            catch (UnauthorizedAccessException)
            {
                await DisplayAlert(
                    "Storage Error",
                    "The application cannot access its account files.",
                    "OK");
            }
        }
    }
}
