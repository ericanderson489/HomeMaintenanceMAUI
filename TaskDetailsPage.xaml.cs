using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI {
  // NEW FILE: No TaskDetailsPage class existed on the branch before these local GUI changes.
  // The page now displays a clicked task, opens its editor, and refreshes after a saved edit.
  /// <summary>
  /// Displays a selected task and opens its editor without accessing task storage directly.
  /// </summary>
  public partial class TaskDetailsPage : ContentPage {
    /// <summary>Prevents repeated Close clicks from navigating back more than once.</summary>
    private bool _isClosing;

    /// <summary>The latest saved snapshot, reused when opening the editor again.</summary>
    private TaskRecord _task;

    /// <summary>Prevents repeated Edit clicks from opening more than one editor.</summary>
    private bool _isOpeningEditor;

    /// <summary>Copies the selected task's display values into the read-only job sheet.</summary>
    /// <param name="task">The existing record supplied by the task list.</param>
    internal TaskDetailsPage(TaskRecord task) {
      InitializeComponent();
      _task = task;
      // BEFORE: The six label assignments now in ShowTask were directly in this constructor.
      // AFTER: ShowTask(task); also runs after a successful edit, refreshing the same labels.
      ShowTask(task);
    }

    /// <summary>Refreshes the read-only fields and retains the snapshot for the next edit.</summary>
    /// <param name="task">The selected record or its successfully saved replacement.</param>
    private void ShowTask(TaskRecord task) {
      _task = task;
      TaskNameLabel.Text = task.Name;
      DescriptionLabel.Text = string.IsNullOrWhiteSpace(task.Description)
          ? "No notes added."
          : task.Description;
      DueDateLabel.Text = task.DueDate.ToString("MMM d, yyyy");
      StatusLabel.Text = task.Status.ToString();
      TaskTypeLabel.Text = task.Type;
      RepeatLabel.Text = task.Repeat.ToString();
    }

    /// <summary>Opens the existing task in the editor and refreshes details after it saves.</summary>
    private async void OnEditTaskClicked(object sender, EventArgs e) {
      // BEFORE: EditTaskButton had IsEnabled="False" and no Clicked handler.
      // AFTER: Clicked="OnEditTaskClicked" opens new TaskEditorPage(_task), so Save has the
      // existing record's ID. This new handler does not create or write any task itself.
      if (_isOpeningEditor || _isClosing) {
        return;
      }
      _isOpeningEditor = true;
      EditTaskButton.IsEnabled = false;
      CloseButton.IsEnabled = false;
      try {
        var editor = new TaskEditorPage(_task);
        editor.TaskSaved += ShowTask;
        await Navigation.PushAsync(editor);
      } finally {
        _isOpeningEditor = false;
        EditTaskButton.IsEnabled = true;
        CloseButton.IsEnabled = true;
      }
    }

    /// <summary>Returns to the task list without saving or modifying the selected record.</summary>
    private async void OnCloseClicked(object sender, EventArgs e) {
      // BEFORE: if (_isClosing) {
      // AFTER: if (_isClosing || _isOpeningEditor) { avoids closing during an Edit transition.
      if (_isClosing || _isOpeningEditor) {
        return;
      }

      _isClosing = true;
      CloseButton.IsEnabled = false;
      try {
        await Navigation.PopAsync();
      } finally {
        _isClosing = false;
        CloseButton.IsEnabled = true;
      }
    }
  }
}
