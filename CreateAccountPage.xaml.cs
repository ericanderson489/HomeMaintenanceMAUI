using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Services;

namespace HomeMaintenanceMAUI;

public partial class CreateAccountPage : ContentPage
{
    // Handles saving and loading accounts from JSON files.
    private readonly AccountStore accountStore = new();
    public CreateAccountPage()
	{
		InitializeComponent();
	}
    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {   // Navigate back to the previous page, which is the login page.
        // The ".." means "go back one page in the navigation stack".
        await Shell.Current.GoToAsync("..");
    }
    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        // Retrieve the information entered by the user.
        string firstName = FirstNameEntry.Text?.Trim() ?? "";
        string lastName = LastNameEntry.Text?.Trim() ?? "";
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";
        // Check whether any required fields are empty.
        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {   // If any required field is empty, show an alert and return early.
            await DisplayAlert(
                "Missing Information",
                "Please fill out all required fields.",
                "OK");

            return;
        }
        // Make sure the two passwords match.
        if (password != confirmPassword)
        {   // If the passwords do not match, show an alert and return early.
            await DisplayAlert(
                "Password Mismatch",
                "The passwords you entered do not match.",
                "OK");

            return;
        }
        // Log the validated username for debugging purposes.
        System.Diagnostics.Debug.WriteLine(
            $"Registration validated for username: {username}");
        try
        {
            // Create an account object using the validated input.
            // The Account constructor hashes the password.
            Account account = new Account(
                username,
                password,
                firstName,
                lastName);
            // Save the account information to a JSON file.
            accountStore.CreateAccount(account);
            // Show a success message to the user.
            await DisplayAlert(
                "Account Created",
                "Your account has been created successfully!",
                "OK");
            // Return to the login page after registration.
            // The ".." means "go back one page in the navigation stack".
            // Shell is a navigation framework in .NET MAUI that allows for easy page transitions.
            await Shell.Current.GoToAsync("..");
        }
        catch (ArgumentException ex)
        {
            // Handles invalid account information or duplicate usernames.
            await DisplayAlert("Registration Error", ex.Message, "OK");
        }
        catch (IOException)
        {
            // Handles problems writing the account file.
            await DisplayAlert(
                "Storage Error",
                "The account could not be saved. Please try again.",
                "OK");
        }
        catch (UnauthorizedAccessException)
        {   // Handles permission issues when writing the account file.
            await DisplayAlert(
                "Storage Error",
                "The application does not have permission to save the account.",
                "OK");
        }
    }
}