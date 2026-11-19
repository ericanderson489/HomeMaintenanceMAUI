using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop table and footer without loading or changing tasks.</summary>
  public partial class TaskListView : ContentView {
    /// <summary>Holds the selected display record for the footer's details action.</summary>
    private TaskRecord? _selectedTask;
    /// <summary>Prevents multiple pages from opening for rapid repeated row clicks.</summary>
    private bool _isOpeningDetails;

    // BEFORE: The table had no retained source list or selected ordering field.
    // AFTER: Keep a GUI-only snapshot and selected sort; account storage remains untouched.
    /// <summary>Retains the records last supplied by the existing list-loading code.</summary>
    private IReadOnlyList<TaskRecord> _sourceTasks = Array.Empty<TaskRecord>();

    /// <summary>Remembers this table's display order when its records are refreshed.</summary>
    private WorkshopTaskSort _sortOrder = WorkshopTaskSort.DueDate;

    /// <summary>Loads the task rows, selection hint, and initially inactive details action.</summary>
    public TaskListView() {
      InitializeComponent();
    }

    /// <summary>Gets or sets the empty-state caption without consulting a task source.</summary>
    public string EmptyMessage {
      get { return EmptyMessageLabel.Text; }
      set { EmptyMessageLabel.Text = value; }
    }
    /// <summary>Displays the supplied records and clears selection when the list is refreshed.</summary>
    internal void SetTasks(IReadOnlyList<TaskRecord> tasks) {
      // BEFORE: The first executable line was TaskRows.ItemsSource = tasks;
      // AFTER: Prepend the two lines below, then keep the team's original ItemsSource and
      // empty-message assignments unchanged. Sorting copies references, not task values:
      // _sourceTasks = tasks.ToArray();
      // tasks = WorkshopTaskOrdering.Order(_sourceTasks, _sortOrder);
      _sourceTasks = tasks.ToArray();
      tasks = WorkshopTaskOrdering.Order(_sourceTasks, _sortOrder);
      // BEFORE: SetTasks only assigned these two display values:
      // TaskRows.ItemsSource = tasks;
      // EmptyMessageLabel.IsVisible = tasks.Count == 0;
      // AFTER: Keep both assignments and clear the previous selection with:
      // TaskRows.SelectedItem = null;
      // _selectedTask = null;
      // ViewDetailsButton.IsEnabled = false;
      // SelectionHintLabel.IsVisible = tasks.Count > 0;
      TaskRows.ItemsSource = tasks;
      EmptyMessageLabel.IsVisible = tasks.Count == 0;
      TaskRows.SelectedItem = null;
      _selectedTask = null;
      ViewDetailsButton.IsEnabled = false;
      SelectionHintLabel.IsVisible = tasks.Count > 0;
    }

    /// <summary>Changes this table's order without reloading or writing task storage.</summary>
    /// <param name="order">The ordering requested by this table's header control.</param>
    internal void SetSortOrder(WorkshopTaskSort order) {
      // BEFORE: No SetSortOrder method existed.
      // AFTER: Reuse the GUI refresh with the cached records and the selected display order.
      if (_sortOrder == order) {
        return;
      }
      _sortOrder = order;
      SetTasks(_sourceTasks);
    }

    /// <summary>Keeps headings and rows aligned at a readable width when the viewport resizes.</summary>
    private void OnTaskViewportSizeChanged(object? sender, EventArgs e) {
      // BEFORE: No viewport-size handler existed. The XAML table used:
      // MinimumWidthRequest="640"
      // WidthRequest="{Binding Width, Source={x:Reference TaskTableViewport}}"
      // This let children measure at the narrow viewport width before arranging wider.
      // AFTER: Set the final width before child measurement and let the horizontal
      // viewport expose wider columns. This changes layout only, never task records.
      if (sender is not ScrollView viewport || TaskTable is null ||
          !double.IsFinite(viewport.Width)) {
        return;
      }

      TaskTable.WidthRequest = Math.Max(640, viewport.Width);
    }

    /// <summary>Tracks the selected row for the footer action without modifying its record.</summary>
    private void OnTaskSelectionChanged(object? sender, SelectionChangedEventArgs e) {
      // NEW: No selection handler existed. The selected row now enables View details.
      _selectedTask = e.CurrentSelection.FirstOrDefault() as TaskRecord;
      ViewDetailsButton.IsEnabled = _selectedTask is not null && !_isOpeningDetails;
    }

    /// <summary>Opens a double-clicked row; a single click only selects it.</summary>
    private async void OnTaskTapped(object? sender, TappedEventArgs e) {
      // BEFORE: <TapGestureRecognizer Tapped="OnTaskTapped" CommandParameter="{Binding .}" />
      // AFTER: The recognizer also sets NumberOfTapsRequired="2". This existing handler's
      // executable body is unchanged; a single click never calls it, but selection remains.
      if (e.Parameter is TaskRecord task) {
        TaskRows.SelectedItem = task;
        await OpenTaskDetailsAsync(task);
      }
    }

    /// <summary>Opens the selected record when the footer button is activated.</summary>
    private async void OnViewDetailsClicked(object? sender, EventArgs e) {
      // NEW: View details previously had no Clicked handler and stayed disabled.
      if (_selectedTask is not null) {
        await OpenTaskDetailsAsync(_selectedTask);
      }
    }

    /// <summary>Shows a read-only page and guards against duplicate navigation from rapid clicks.</summary>
    private async Task OpenTaskDetailsAsync(TaskRecord task) {
      // NEW: No details navigation existed. Push the new page with the selected record.
      if (_isOpeningDetails) {
        return;
      }

      _isOpeningDetails = true;
      ViewDetailsButton.IsEnabled = false;
      try {
        // Pass the existing immutable record directly; no account or task data is written.
        await Navigation.PushAsync(new TaskDetailsPage(task));
      } finally {
        _isOpeningDetails = false;
        ViewDetailsButton.IsEnabled = _selectedTask is not null;
      }
    }
  }
}
