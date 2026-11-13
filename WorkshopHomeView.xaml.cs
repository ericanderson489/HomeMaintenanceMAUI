using HomeMaintenanceMAUI.Models.Records; // Added 11/11/26
using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Services;

namespace HomeMaintenanceMAUI {
  /// <summary>Displays task totals and upcoming jobs from records supplied by the dashboard.</summary>
  public partial class WorkshopHomeView : ContentView {
    /// <summary>Requests presentation-only navigation to the Tasks screen.</summary>
    public event EventHandler? ViewTasksRequested;
    /// <summary>Loads headings, initially empty summary cards, and the upcoming-jobs table.</summary>
    public WorkshopHomeView() {
      InitializeComponent();
    }
    /// <summary>Opens the existing Add task editor without changing its save behavior.</summary>
    private async void OnAddTaskClicked(object sender, EventArgs e) {
      await Shell.Current.GoToAsync(nameof(TaskEditorPage));
    }

    /// <summary>Requests navigation to the existing Tasks view.</summary>
    private void OnViewTasksClicked(object sender, EventArgs e) {
      ViewTasksRequested?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>Displays the supplied active tasks due today or later.</summary>
    /// <param name="activeTasks">The dashboard's existing pending, unarchived task list.</param>
    internal void SetTasks(IReadOnlyList<TaskRecord> activeTasks) {
      // Upcoming Jobs shows tasks due today or later.
      var upcoming = activeTasks
          .Where(task => task.DueDate.Date >= DateTime.Today)
          .ToList();
      UpcomingJobs.SetTasks(upcoming);
    }

    /// <summary>Refreshes the four display totals without changing or saving any task.</summary>
    /// <param name="tasks">All records from the dashboard's existing account load.</param>
    internal void SetTaskSummary(IReadOnlyList<TaskRecord> tasks) {
      // BEFORE: There was no summary method. The XAML contained these fixed values:
      // Text="0  On the bench"
      // Text="0  Due soon"
      // Text="0  Overdue"
      // Text="0  Finished"
      // AFTER: The four label assignments below display counts from the supplied records.
      // On the bench includes all pending, unarchived tasks, including overdue ones.
      int onBench = tasks.Count(task => !task.IsArchived && task.Status == Status.Pending);
      // Reuse the team's existing date rules: today through seven days ahead, inclusive,
      // and dates before today. Both helpers already exclude completed/archived tasks.
      int dueSoon = MaintenanceReminderService.DueWithinSevenDays(tasks).Count;
      int overdue = MaintenanceReminderService.OverdueTasks(tasks).Count;
      // Completed records stay in Finished only while they are waiting to be archived.
      int finished = tasks.Count(task => !task.IsArchived && task.Status == Status.Complete);

      // AFTER (exact display replacements):
      // OnBenchSummaryLabel.Text = $"{onBench}  On the bench";
      // DueSoonSummaryLabel.Text = $"{dueSoon}  Due soon";
      // OverdueSummaryLabel.Text = $"{overdue}  Overdue";
      // FinishedSummaryLabel.Text = $"{finished}  Finished";
      OnBenchSummaryLabel.Text = $"{onBench}  On the bench";
      DueSoonSummaryLabel.Text = $"{dueSoon}  Due soon";
      OverdueSummaryLabel.Text = $"{overdue}  Overdue";
      FinishedSummaryLabel.Text = $"{finished}  Finished";
    }
  }
}
