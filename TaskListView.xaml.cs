using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop table and footer without loading or changing tasks.</summary>
  public partial class TaskListView : ContentView {
    /// <summary>Holds the selected display record for the footer's details action.</summary>
    private TaskRecord? _selectedTask;
    /// <summary>Prevents multiple pages from opening for rapid repeated row clicks.</summary>
    private bool _isOpeningDetails;

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

    /// <summary>Tracks the selected row for the footer action without modifying its record.</summary>
    private void OnTaskSelectionChanged(object? sender, SelectionChangedEventArgs e) {
      // NEW: No selection handler existed. The selected row now enables View details.
      _selectedTask = e.CurrentSelection.FirstOrDefault() as TaskRecord;
      ViewDetailsButton.IsEnabled = _selectedTask is not null && !_isOpeningDetails;
    }

    /// <summary>Opens the clicked row, including a row clicked again after closing its details.</summary>
    private async void OnTaskTapped(object? sender, TappedEventArgs e) {
      // NEW: No row-tap handler existed. Pass the clicked record to the details GUI.
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
