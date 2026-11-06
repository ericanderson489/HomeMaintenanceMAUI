namespace HomeMaintenanceMAUI {
  /// <summary>Displays the overview without computing counts or reading stored tasks.</summary>
  public partial class WorkshopHomeView : ContentView {
    /// <summary>Requests presentation-only navigation to the Tasks screen.</summary>
    public event EventHandler? ViewTasksRequested;

    /// <summary>Loads headings, unbound summary cards, and the upcoming-jobs table.</summary>
    public WorkshopHomeView() {
      InitializeComponent();
    }

    /// <summary>Opens the task form for layout review; the form has no save action.</summary>
    private async void OnAddTaskClicked(object sender, EventArgs e) {
      var button = (Button)sender;
      button.IsEnabled = false;
      try {
        await Navigation.PushModalAsync(new NavigationPage(new TaskEditorPage()));
      } finally {
        button.IsEnabled = true;
      }
    }

    /// <summary>Asks the workspace to show Tasks without consulting task data.</summary>
    private void OnViewTasksClicked(object sender, EventArgs e) {
      ViewTasksRequested?.Invoke(this, EventArgs.Empty);
    }
  }
}
