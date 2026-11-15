using HomeMaintenanceMAUI.Models.Records; // Added 11/11/26
using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Services;
using Microsoft.Maui.Dispatching;

namespace HomeMaintenanceMAUI {
  /// <summary>Displays task totals and upcoming jobs from records supplied by the dashboard.</summary>
  public partial class WorkshopHomeView : ContentView {
    // BEFORE: No summary snapshot, date, or timer fields existed.
    // AFTER: These GUI-only fields retain the last supplied records and refresh their counts
    // after a date change. They never load/save accounts or refresh the team's task lists.
    /// <summary>Retains immutable records last supplied to our summary display.</summary>
    private IReadOnlyList<TaskRecord> _summaryTasks = Array.Empty<TaskRecord>();

    /// <summary>Records the local date used by the last summary update.</summary>
    private DateTime _summaryDate = DateTime.MinValue;

    /// <summary>Checks for a date change only while this view is loaded.</summary>
    private IDispatcherTimer? _summaryTimer;

    /// <summary>Requests presentation-only navigation to the Tasks screen.</summary>
    public event EventHandler? ViewTasksRequested;
    /// <summary>Loads headings, initially empty summary cards, and the upcoming-jobs table.</summary>
    public WorkshopHomeView() {
      // BEFORE: InitializeComponent(); was the constructor's complete body.
      // AFTER: Keep InitializeComponent(); and add Loaded/Unloaded subscriptions below.
      InitializeComponent();
      Loaded += OnSummaryLoaded;
      Unloaded += OnSummaryUnloaded;
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
      // BEFORE: The first executable line was:
      // int onBench = tasks.Count(task => !task.IsArchived && task.Status == Status.Pending);
      // AFTER: First copy the supplied snapshot and remember its display date, then run
      // the same four count calculations and label assignments without changing them.
      _summaryTasks = tasks.ToArray();
      _summaryDate = DateTime.Today;
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

    /// <summary>Starts our display-only date check when the Home view enters the window.</summary>
    private void OnSummaryLoaded(object? sender, EventArgs e) {
      // BEFORE: No OnSummaryLoaded method or date-check timer existed.
      // AFTER: Create one timer, refresh stale counts immediately, and check every 30 seconds.
      if (_summaryTimer is null) {
        _summaryTimer = Dispatcher.CreateTimer();
        _summaryTimer.Interval = TimeSpan.FromSeconds(30);
        _summaryTimer.Tick += OnSummaryTimerTick;
      }
      OnSummaryTimerTick(this, EventArgs.Empty);
      _summaryTimer.Start();
    }

    /// <summary>Releases the date-check timer when the Home view leaves the window.</summary>
    private void OnSummaryUnloaded(object? sender, EventArgs e) {
      // BEFORE: No OnSummaryUnloaded method or timer cleanup existed.
      // AFTER: Stop the timer and remove its callback so it cannot retain an old dashboard.
      if (_summaryTimer is null) {
        return;
      }
      _summaryTimer.Stop();
      _summaryTimer.Tick -= OnSummaryTimerTick;
      _summaryTimer = null;
    }

    /// <summary>Recalculates only our four counters when the local calendar date changes.</summary>
    private void OnSummaryTimerTick(object? sender, EventArgs e) {
      // BEFORE: Counts changed only when DashboardPage supplied records on page appearance.
      // AFTER: Reuse SetTaskSummary with its cached records when the calendar date changes.
      // The team's SetTasks methods, upcoming-date filter, and storage reads remain untouched.
      if (_summaryDate != DateTime.Today) {
        SetTaskSummary(_summaryTasks);
      }
    }
  }
}
