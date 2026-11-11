namespace HomeMaintenanceMAUI {
  /// <summary>
  /// Adds the original login screen's Cancel action without changing authentication.
  /// </summary>
  public partial class MainPage {
    /// <summary>Closes the application when the user cancels the sign-in screen.</summary>
    private void OnCancelClicked(object sender, EventArgs e) {
      Application.Current?.Quit();
    }
  }
}
