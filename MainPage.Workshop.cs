namespace HomeMaintenanceMAUI {
  /// <summary>
  /// Adds the original login screen's Cancel action without changing authentication.
  /// </summary>
  public partial class MainPage {
    /// <summary>Closes the application when the user cancels the sign-in screen.</summary>
    private void OnCancelClicked(object sender, EventArgs e) {
      Application.Current?.Quit();
    }

    /// <summary>Clears the cached login fields when the Workshop GUI signs out.</summary>
    // BEFORE: no ClearWorkshopCredentials method existed in this GUI partial class.
    // AFTER: the new GUI-only method is exactly:
    // internal void ClearWorkshopCredentials() {
    //   UsernameEntry.Text = string.Empty;
    //   PasswordEntry.Text = string.Empty;
    // }
    internal void ClearWorkshopCredentials() {
      UsernameEntry.Text = string.Empty;
      PasswordEntry.Text = string.Empty;
    }
  }
}
