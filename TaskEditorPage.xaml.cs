using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Services;
using HomeMaintenanceMAUI.Models.Records; // Added 11/11/26

namespace HomeMaintenanceMAUI;
/// <summary>
/// Validates the task form and either adds a task or updates the selected existing task.
/// </summary>
public partial class TaskEditorPage : ContentPage {
  /// <summary>The original record; null keeps the existing Add task save path.</summary>
  private readonly TaskRecord? _originalTask;

  /// <summary>The account that opened an existing task, used to prevent cross-account edits.</summary>
  private readonly string? _editingUsername;

  /// <summary>Prevents repeated Save clicks while validation, alerts, or navigation are pending.</summary>
  private bool _isSaving;

  /// <summary>Notifies the details GUI after the edited record is successfully saved.</summary>
  internal event Action<TaskRecord>? TaskSaved;

  /// <summary>Initializes an empty Add task form suitable for modal presentation.</summary>
  public TaskEditorPage() : this(false) {
  }

  /// <summary>
  /// Initializes picker choices and the form caption; the record overload supplies edit values.
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

  /// <summary>Opens the selected task with its current values and preserves its original ID.</summary>
  /// <param name="task">The immutable snapshot displayed by the details page.</param>
  internal TaskEditorPage(TaskRecord task) : this(true) {
    // BEFORE: The bool constructor only changed these captions and loaded no task values:
    // Title = isEditing ? "Edit task" : "Add task";
    // EditorHeading.Text = isEditing ? "EDIT TASK" : "ADD TASK";
    // AFTER: That constructor is unchanged. This new overload retains the selected record
    // in _originalTask and fills the five controls below before the user edits them.
    _originalTask = task;
    _editingUsername = UserSession.CurrentAccount?.GetAccountUserName();
    TaskTitleEntry.Text = task.Name;
    TaskNotesEditor.Text = task.Description;
    DueDatePicker.Date = task.DueDate;
    // NEW: Keep an unrecognized stored type selectable instead of replacing it or throwing.
    // No task-loading code existed before this overload; Add keeps its original enum choices.
    List<string> types = Enum.GetNames<Category>().ToList();
    if (!types.Contains(task.Type)) {
      types.Add(task.Type);
    }
    TaskTypePicker.ItemsSource = types;
    TaskTypePicker.SelectedItem = task.Type;
    if (!Enum.IsDefined(task.Repeat)) {
      RepeatPicker.ItemsSource = Enum.GetValues<Frequency>().Append(task.Repeat).ToList();
    }
    RepeatPicker.SelectedItem = task.Repeat;
  }

  /// <summary>
  /// Closes the modal form, discarding only the inputs held by its native controls.
  /// </summary>
  private async void OnCancelClicked(object sender, EventArgs e) {
    await Shell.Current.GoToAsync("..");
  }
  /// <summary>Runs one save at a time so repeated clicks cannot submit or close the form twice.</summary>
  private async void OnSaveClicked(object sender, EventArgs e) {
    // BEFORE: private async void OnSaveClicked(object sender, EventArgs e) {
    // AFTER: private async Task SaveTaskAsync() {
    // The existing save body moved to that method below; this event handler now awaits it
    // inside a new _isSaving guard so repeated clicks do not submit the form twice.
    if (_isSaving) {
      return;
    }
    _isSaving = true;
    try {
      await SaveTaskAsync();
    } finally {
      _isSaving = false;
    }
  }

  /// <summary>Preserves existing validation and selects the add or existing-task save path.</summary>
  private async Task SaveTaskAsync() {
    var errors = new List<string>();
    // Check for null, empty, or whitespace-only text
    if (string.IsNullOrWhiteSpace(TaskTitleEntry.Text))
      errors.Add("• Task title is required.");
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
    string desc = TaskNotesEditor.Text?.Trim() ?? string.Empty;
    DateTime date = DueDatePicker.Date.Date;
    // BEFORE: Category type = (Category)TaskTypePicker.SelectedItem;
    // AFTER: string type = TaskTypePicker.SelectedItem.ToString()!;
    // Both save the same enum name for normal choices; an existing unknown type is retained.
    string type = TaskTypePicker.SelectedItem.ToString()!;
    Frequency freq = (Frequency)RepeatPicker.SelectedItem;
    // Save task to user account
    var account = UserSession.CurrentAccount;
    if (account is null) {
      await DisplayAlert("Sign-in required", "Sign in again to save a task.", "OK");
      return;
    }
    // NEW: No account check for editing existed. Only the account that opened this record
    // may update it; the original Add task sign-in check above remains unchanged.
    if (_originalTask is not null &&
        !string.Equals(_editingUsername, account.GetAccountUserName(),
            StringComparison.OrdinalIgnoreCase)) {
      await DisplayAlert("Sign-in changed", "Reopen this task from your account to edit it.", "OK");
      return;
    }

    // BEFORE (every save created a new ID and reset the status):
    // var task = new TaskRecord(
    //     Id: Guid.NewGuid(),
    //     Name: name,
    //     Description: desc,
    //     Type: type.ToString(),
    //     DueDate: date,
    //     Status: Status.Pending,
    //     Repeat: freq);
    // AFTER (the replacement expression below):
    // TaskRecord task = _originalTask is null ? new TaskRecord(
    //     Id: Guid.NewGuid(),
    //     Name: name,
    //     Description: desc,
    //     Type: type.ToString(),
    //     DueDate: date,
    //     Status: Status.Pending,
    //     Repeat: freq) : _originalTask with {
    //   Name = name,
    //   Description = desc,
    //   Type = type.ToString(),
    //   DueDate = date,
    //   Repeat = freq
    // };
    // Add keeps the original defaults; Edit copies the record without resetting its ID/history.
    TaskRecord task = _originalTask is null ? new TaskRecord(
        Id: Guid.NewGuid(),
        Name: name,
        Description: desc,
        Type: type.ToString(),
        DueDate: date,
        Status: Status.Pending,
        Repeat: freq) : _originalTask with {
      Name = name,
      Description = desc,
      Type = type.ToString(),
      DueDate = date,
      Repeat = freq
    };
    try {
      // BEFORE:
      // new AccountStore().AddTask(account.GetAccountUserName(), task);
      // AFTER (replacement below):
      // if (_originalTask is null) {
      //   new AccountStore().AddTask(account.GetAccountUserName(), task);
      // } else {
      //   task = new AccountStore().UpdateTask(account.GetAccountUserName(), _originalTask, task);
      // }
      if (_originalTask is null) {
        new AccountStore().AddTask(account.GetAccountUserName(), task);
      } else {
        task = new AccountStore().UpdateTask(account.GetAccountUserName(), _originalTask, task);
      }
    } catch (InvalidOperationException ex) when (_originalTask is not null) {
      // NEW: A stale or deleted task is never appended or overwritten; leave the form open.
      await DisplayAlert("Task changed", ex.Message, "OK");
      return;
    } catch (Exception ex) when (
          ex is IOException or UnauthorizedAccessException or InvalidDataException) {
      System.Diagnostics.Debug.WriteLine($"Unable to save task: {ex}");
      await DisplayAlert("Save failed", "The account file could not be updated. Please try again.", "OK");
      return;
    }
    // BEFORE:
    // await DisplayAlert("Task saved", "The task has been added to your account.", "OK");
    // AFTER: Notify the details GUI only after persistence succeeds, then use the right message:
    // TaskSaved?.Invoke(task);
    // string message = _originalTask is null
    //     ? "The task has been added to your account."
    //     : "The task has been updated.";
    // await DisplayAlert("Task saved", message, "OK");
    TaskSaved?.Invoke(task);
    string message = _originalTask is null
        ? "The task has been added to your account."
        : "The task has been updated.";
    await DisplayAlert("Task saved", message, "OK");
    // Returns to previous page
    await Shell.Current.GoToAsync("..");
  }
}
