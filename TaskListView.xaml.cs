using HomeMaintenanceMAUI.Models.Records;
using HomeMaintenanceMAUI.Models.Records; // Added 11/11/26
namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop table and footer without loading or changing tasks.</summary>
  public partial class TaskListView : ContentView {
    public TaskListView() {
      InitializeComponent();
    }
    /// <summary>Gets or sets the empty-state caption without consulting a task source.</summary>
    /// /// <summary>Loads the column headings, blank body, and inactive details action.</summary>
    public string EmptyMessage {
      get { return EmptyMessageLabel.Text; }
      set { EmptyMessageLabel.Text = value; }
    }
    
    internal void SetTasks(IReadOnlyList<TaskRecord> tasks) {
      TaskRows.ItemsSource = tasks;
      EmptyMessageLabel.IsVisible = tasks.Count == 0;
    }
  }
}
