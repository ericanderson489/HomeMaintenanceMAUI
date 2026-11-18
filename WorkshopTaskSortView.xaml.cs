namespace HomeMaintenanceMAUI {
  // BEFORE: No WorkshopTaskSortView class or task-order selection handler existed.
  // AFTER: This control forwards a GUI ordering choice to its existing task table.
  /// <summary>Lets a Home or Tasks header choose the order of its displayed records.</summary>
  public partial class WorkshopTaskSortView : ContentView {
    /// <summary>Identifies the table controlled by this header's sorting picker.</summary>
    public static readonly BindableProperty TargetListProperty = BindableProperty.Create(
        nameof(TargetList), typeof(TaskListView), typeof(WorkshopTaskSortView), null,
        propertyChanged: OnTargetListChanged);

    /// <summary>Gets or sets the existing task table whose display order should change.</summary>
    public TaskListView? TargetList {
      get { return (TaskListView?)GetValue(TargetListProperty); }
      set { SetValue(TargetListProperty, value); }
    }

    /// <summary>Loads the three GUI choices and defaults to earliest due date first.</summary>
    public WorkshopTaskSortView() {
      InitializeComponent();
      SortPicker.SelectedIndex = 0;
    }

    /// <summary>Applies the displayed choice when XAML supplies the target task table.</summary>
    private static void OnTargetListChanged(
        BindableObject bindable, object oldValue, object newValue) {
      ((WorkshopTaskSortView)bindable).ApplySelection();
    }

    /// <summary>Updates only the target table when the user changes its ordering choice.</summary>
    private void OnSortChanged(object? sender, EventArgs e) {
      ApplySelection();
    }

    /// <summary>Maps picker choices to the corresponding presentation-only ordering.</summary>
    private void ApplySelection() {
      if (SortPicker.SelectedIndex < 0) {
        return;
      }
      TargetList?.SetSortOrder((WorkshopTaskSort)SortPicker.SelectedIndex);
    }
  }
}
