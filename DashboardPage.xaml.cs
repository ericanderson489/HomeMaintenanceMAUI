using HomeMaintenanceMAUI.Services;
using HomeMaintenanceMAUI.Models; // Added 11/11/26

namespace HomeMaintenanceMAUI {
  /// <summary>Hosts the Workshop screens reached by the existing successful-login route.</summary>
  public partial class DashboardPage : ContentPage {
    /// <summary>
    /// Loads the workspace and selects Home without reading or changing account tasks.
    /// </summary>
    public DashboardPage() {
      InitializeComponent();
      ShowWorkspace(HomeView);
    }
    // 
    protected override async void OnAppearing() {
      // Reloads the data when the dashboard comes back into view
      base.OnAppearing();

      var account = UserSession.CurrentAccount;
      if (account is null)
        return;

      try {
        // Reload the account's tasks on sign-in and when returning from Add task.
        var activeTasks = new AccountStore()
            .LoadTasks(account.GetAccountUserName())
            .Where(task => !task.IsArchived && task.Status == Status.Pending)
            .OrderBy(task => task.DueDate)
            .ToList();

        TasksView.SetTasks(activeTasks);
        HomeView.SetTasks(activeTasks);
      } catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException or InvalidDataException) {
        System.Diagnostics.Debug.WriteLine($"Unable to load dashboard tasks: {ex}");
        await DisplayAlert("Tasks unavailable", "The saved tasks could not be loaded.", "OK");
      }
    }

    /// <summary>Displays Home in response to desktop or compact navigation.</summary>
    private void OnHomeClicked(object? sender, EventArgs e) {
      ShowWorkspace(HomeView);
    }

    /// <summary>Displays the task layout without loading, filtering, or mutating tasks.</summary>
    private void OnTasksClicked(object? sender, EventArgs e) {
      ShowWorkspace(TasksView);
    }

    /// <summary>
    /// Displays the Renovations placeholder without loading or changing project data.
    /// </summary>
    private void OnRenovationsClicked(object? sender, EventArgs e) {
      ShowWorkspace(RenovationsView);
    }

    /// <summary>
    /// Returns to the existing login route and removes the workspace navigation entry.
    /// Sets CurrentAccount to null so that it does not live on in memory as a static variable.
    /// </summary>
    private async void OnSignOutClicked(object sender, EventArgs e) {
      UserSession.CurrentAccount = null;
      await Shell.Current.GoToAsync("//MainPage");
    }

    /// <summary>
    /// Updates screen visibility and selected navigation colors for both layouts.
    /// </summary>
    private void ShowWorkspace(ContentView selectedView) {
      bool showHome = selectedView == HomeView;
      bool showTasks = selectedView == TasksView;
      bool showRenovations = selectedView == RenovationsView;
      HomeView.IsVisible = showHome;
      TasksView.IsVisible = showTasks;
      RenovationsView.IsVisible = showRenovations;
      SetNavigationSelection(HomeButton, showHome);
      SetNavigationSelection(CompactHomeButton, showHome);
      SetNavigationSelection(TasksButton, showTasks);
      SetNavigationSelection(CompactTasksButton, showTasks);
      SetNavigationSelection(RenovationsButton, showRenovations);
      SetNavigationSelection(CompactRenovationsButton, showRenovations);
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
