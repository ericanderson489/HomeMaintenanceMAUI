namespace HomeMaintenanceMAUI;

/// <summary>
/// Provides the add/edit task form without validation, task mutation, or persistence.
/// </summary>
public partial class TaskEditorPage : ContentPage {
  private bool _isClosing;

  /// <summary>Initializes an empty Add task form suitable for modal presentation.</summary>
  public TaskEditorPage() : this(false) {
  }

  /// <summary>
  /// Initializes the form caption for adding or editing; neither mode accepts a task record.
  /// </summary>
  /// <param name="isEditing">Whether to display the Edit task caption instead of Add task.</param>
  public TaskEditorPage(bool isEditing) {
    InitializeComponent();
    Title = isEditing ? "Edit task" : "Add task";
    EditorHeading.Text = isEditing ? "EDIT TASK" : "ADD TASK";
  }

  /// <summary>
  /// Closes the modal form, discarding only the inputs held by its native controls.
  /// </summary>
  private async void OnCancelClicked(object sender, EventArgs e) {
    if (_isClosing) {
      return;
    }

    _isClosing = true;
    try {
      await Navigation.PopModalAsync();
    } finally {
      _isClosing = false;
    }
  }
}
