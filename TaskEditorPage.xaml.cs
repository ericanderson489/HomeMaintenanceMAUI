using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Services;

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
    await Shell.Current.GoToAsync("..");
  }

  private async void OnSaveClicked(object sender, EventArgs e) {
    var errors = new List<string>();

    // Check for null, empty, or whitespace-only text
    if (string.IsNullOrWhiteSpace(TaskTitleEntry.Text))
      errors.Add("• Task title is required.");

    if (string.IsNullOrWhiteSpace(TaskNotesEditor.Text))
      errors.Add("• Task notes are required.");

    // Validate the date 
    if (DueDatePicker.Date.Date <= DateTime.Today)
      errors.Add("• Due date must be after today.");

    // Stop execution and show if any check failed/what user can do to fix
    if (errors.Count > 0) {
      await DisplayAlert("Entry Failed", 
        string.Join("\n", errors), 
        "OK");
      return; // Prevents saving invalid data below
    }

    // Capture input
    string name = TaskTitleEntry.Text!.Trim();
    string desc = TaskNotesEditor.Text!.Trim();
    DateTime date = DueDatePicker.Date.Date;
    Category type = (Category)TaskTypePicker.SelectedItem;
    Frequency freq = (Frequency)RepeatPicker.SelectedItem;

    // Save task to user account
    Tasks task = new Tasks(name, desc, date, type, freq);
    UserSession.CurrentAccount.SetUserTasks(task);
    await DisplayAlert("Entry Successful", 
      "The task has been added", 
      "OK");
    // Could optionally navigate back to the previous page here as well.
    // await Shell.Current.GoToAsync("..");
  }
}
