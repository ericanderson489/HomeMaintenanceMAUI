namespace HomeMaintenanceMAUI;

/// <summary>
/// Arranges the Workshop task list and job sheet, providing only presentation navigation.
/// </summary>
public partial class TasksView : ContentView {
  // The sidebar consumes approximately 205 pixels in a desktop window.
  private const double _wideLayoutMinimumWidth = 760;

  // A remembered layout prevents repeated grid changes during the resulting size notifications.
  private bool? _isWideLayout;
  private bool _isOpeningEditor;

  /// <summary>Initializes the task-screen layout without connecting any task services.</summary>
  public TasksView() {
    InitializeComponent();
  }

  /// <summary>Opens the task form for display without creating or saving a task.</summary>
  private async void OnAddTaskClicked(object sender, EventArgs e) {
    if (_isOpeningEditor) {
      return;
    }

    _isOpeningEditor = true;
    try {
      await Navigation.PushModalAsync(new NavigationPage(new TaskEditorPage()));
    } finally {
      _isOpeningEditor = false;
    }
  }

  /// <summary>
  /// Stacks the job sheet on narrow windows and places it beside the list on desktop.
  /// </summary>
  private void OnSizeChanged(object sender, EventArgs e) {
    if (WorkspaceGrid is null) {
      return;
    }

    bool isWide = Width >= _wideLayoutMinimumWidth;
    if (_isWideLayout == isWide) {
      return;
    }

    _isWideLayout = isWide;
    WorkspaceGrid.ColumnDefinitions.Clear();
    WorkspaceGrid.RowDefinitions.Clear();
    WorkspaceGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
    WorkspaceGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

    if (isWide) {
      WorkspaceGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(300)));
      Grid.SetRow(JobSheet, 0);
      Grid.SetColumn(JobSheet, 1);
    } else {
      WorkspaceGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
      Grid.SetRow(JobSheet, 1);
      Grid.SetColumn(JobSheet, 0);
    }
  }
}
