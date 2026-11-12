using HomeMaintenanceMAUI.Models.Records; // Added 11/11/26

namespace HomeMaintenanceMAUI {
  /// <summary>Displays the overview without computing counts or reading stored tasks.</summary>
  public partial class WorkshopHomeView : ContentView {
    /// <summary>Requests presentation-only navigation to the Tasks screen.</summary>
    public event EventHandler? ViewTasksRequested;

    /// <summary>Loads headings, unbound summary cards, and the upcoming-jobs table.</summary>
    public WorkshopHomeView() {
      InitializeComponent();
    }

    /// <summary>
    /// Shows the requested handoff message without opening a form or changing tasks.
    /// </summary>
    private async void OnAddTaskClicked(object sender, EventArgs e) {
      await Shell.Current.GoToAsync(nameof(TaskEditorPage));
    }

    /// <summary>Asks the workspace to show Tasks without consulting task data.</summary>
    private void OnViewTasksClicked(object sender, EventArgs e) {
      ViewTasksRequested?.Invoke(this, EventArgs.Empty);
    }
    internal void SetTasks(IReadOnlyList<TaskRecord> activeTasks) {
      // Upcoming Jobs shows tasks due today or later.
      var upcoming = activeTasks
          .Where(task => task.DueDate.Date >= DateTime.Today)
          .ToList();

      UpcomingJobs.SetTasks(upcoming);
    }
  }
}
