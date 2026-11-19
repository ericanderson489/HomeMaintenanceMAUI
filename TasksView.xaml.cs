using HomeMaintenanceMAUI.Models.Records; // Added 11/11/26

namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop task list without connecting task services.</summary>
  public partial class TasksView : ContentView {
    /// <summary>Loads the heading, filter control, and full-width empty table.</summary>
    public TasksView() {
      InitializeComponent();
      // Select after XAML populates Items; this only sets the displayed choice, not a task filter.
      JobFilterPicker.SelectedIndex = 0;
    }
    internal void SetTasks(IReadOnlyList<TaskRecord> activeTasks) {
      JobList.SetTasks(activeTasks);
    }
    // Opens the editor
    private async void OnAddTaskClicked(object sender, EventArgs e) {
      await Shell.Current.GoToAsync(nameof(TaskEditorPage));
    }
  }
}
