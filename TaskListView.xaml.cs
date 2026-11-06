namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop table and footer without loading or changing tasks.</summary>
  public partial class TaskListView : ContentView {
    /// <summary>Gets or sets the empty-state caption without consulting a task source.</summary>
    public string EmptyMessage {
      get { return EmptyMessageLabel.Text; }
      set { EmptyMessageLabel.Text = value; }
    }

    /// <summary>Loads the column headings, blank body, and inactive details action.</summary>
    public TaskListView() {
      InitializeComponent();
    }
  }
}
