namespace HomeMaintenanceMAUI {
  /// <summary>Hosts the Workshop screens reached by the existing successful-login route.</summary>
  public partial class DashboardPage : ContentPage {
    /// <summary>
    /// Loads the workspace and selects Home without reading or changing account tasks.
    /// </summary>
    public DashboardPage() {
      InitializeComponent();
      ShowWorkspace(showHome: true);
    }

    /// <summary>Displays Home in response to desktop or compact navigation.</summary>
    private void OnHomeClicked(object? sender, EventArgs e) {
      ShowWorkspace(showHome: true);
    }

    /// <summary>Displays the task layout without loading, filtering, or mutating tasks.</summary>
    private void OnTasksClicked(object? sender, EventArgs e) {
      ShowWorkspace(showHome: false);
    }

    /// <summary>
    /// Returns to the existing login route and removes the workspace navigation entry.
    /// </summary>
    private async void OnSignOutClicked(object sender, EventArgs e) {
      await Shell.Current.GoToAsync("//MainPage");
    }

    /// <summary>
    /// Updates screen visibility and selected navigation colors for both layouts.
    /// </summary>
    private void ShowWorkspace(bool showHome) {
      HomeView.IsVisible = showHome;
      TasksView.IsVisible = !showHome;
      SetNavigationSelection(HomeButton, showHome);
      SetNavigationSelection(CompactHomeButton, showHome);
      SetNavigationSelection(TasksButton, !showHome);
      SetNavigationSelection(CompactTasksButton, !showHome);
    }

    /// <summary>
    /// Applies the original gold outline and ink fill to a selected navigation item.
    /// </summary>
    private void SetNavigationSelection(Button button, bool isSelected) {
      ResourceDictionary resources = Application.Current!.Resources;
      button.BackgroundColor = (Color)resources[isSelected ? "WorkshopInk" : "WorkshopNavy"];
      button.BorderColor = (Color)resources[isSelected ? "WorkshopGold" : "WorkshopMuted"];
    }
  }
}
