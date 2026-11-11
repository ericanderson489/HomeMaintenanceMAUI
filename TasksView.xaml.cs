namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop task list without connecting task services.</summary>
  public partial class TasksView : ContentView {
    /// <summary>Loads the heading, filter control, and full-width empty table.</summary>
    public TasksView() {
      InitializeComponent();
      // Select after XAML populates Items; this only sets the displayed choice, not a task filter.
      JobFilterPicker.SelectedIndex = 0;
    }

    /// <summary>
    /// Shows the requested handoff message without opening a form or changing tasks.
    /// </summary>
    private async void OnAddTaskClicked(object sender, EventArgs e) {
      var button = (Button)sender;
      button.IsEnabled = false;
      try {
        await Shell.Current.DisplayAlert("Add task", "keifer work here.", "OK");
      } finally {
        button.IsEnabled = true;
      }
    }
  }
}
