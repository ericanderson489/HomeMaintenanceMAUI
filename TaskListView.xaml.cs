using HomeMaintenanceMAUI.Models.Records;
namespace HomeMaintenanceMAUI {
  /// <summary>Displays the Workshop table and footer without loading or changing tasks.</summary>
  public partial class TaskListView : ContentView {
    public TaskListView() {
      InitializeComponent();
    }
    /// <summary>Gets or sets the empty-state caption without consulting a task source.</summary>
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
