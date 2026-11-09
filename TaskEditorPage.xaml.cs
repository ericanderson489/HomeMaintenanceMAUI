namespace HomeMaintenanceMAUI;

/// <summary>
/// Provides the add/edit task form without validation, task mutation, or persistence.
/// </summary>
public partial class TaskEditorPage : ContentPage {
  /// <summary>Initializes an empty Add task form suitable for modal presentation.</summary>
  public TaskEditorPage() : this(false) {
  }

  /// <summary>
  /// Initializes the form caption for adding or editing; neither mode accepts a task record.
  /// </summary>
  /// <param name="isEditing">Whether to display the Edit task caption instead of Add task.</param>
  public TaskEditorPage(bool isEditing) {
    InitializeComponent();
    TaskTypePicker.ItemsSource = Enum.GetValues(typeof(Category));
    TaskTypePicker.SelectedIndex = 0;
    RepeatPicker.ItemsSource = Enum.GetValues(typeof(Frequency));
    RepeatPicker.SelectedIndex = 0;
    Title = isEditing ? "Edit task" : "Add task";
    EditorHeading.Text = isEditing ? "EDIT TASK" : "ADD TASK";
  }

  /// <summary>
  /// Closes the modal form, discarding only the inputs held by its native controls.
  /// </summary>
  private async void OnCancelClicked(object sender, EventArgs e) {
    await Shell.Current.GoToAsync(nameof(DashboardPage));
  }

  private void OnSaveClicked(object sender, EventArgs e) {

  }
}
