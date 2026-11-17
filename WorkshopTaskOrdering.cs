using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI {
  // BEFORE: No WorkshopTaskSort enum or WorkshopTaskOrdering helper existed.
  // AFTER: These presentation-only choices order copied task references for a table.
  // The stored TaskRecord values and the caller's original collection are never changed.
  /// <summary>Lists the three available task-table ordering choices.</summary>
  public enum WorkshopTaskSort {
    /// <summary>Shows the earliest due dates first.</summary>
    DueDate,
    /// <summary>Groups task types alphabetically, then shows earlier due dates first.</summary>
    TaskType,
    /// <summary>Groups the existing repeat frequencies, then shows earlier due dates first.</summary>
    RepeatFrequency
  }

  /// <summary>Creates sorted display lists without saving or modifying any task.</summary>
  internal static class WorkshopTaskOrdering {
    /// <summary>Orders the supplied records and returns a new list of the same records.</summary>
    /// <param name="tasks">The latest records supplied to this GUI table.</param>
    /// <param name="order">The order selected by the table's header control.</param>
    /// <returns>A separate display list; the source collection remains unchanged.</returns>
    internal static IReadOnlyList<TaskRecord> Order(
        IReadOnlyList<TaskRecord> tasks, WorkshopTaskSort order) {
      IOrderedEnumerable<TaskRecord> sorted = order switch {
        WorkshopTaskSort.TaskType => tasks
            .OrderBy(task => task.Type, StringComparer.OrdinalIgnoreCase)
            .ThenBy(task => task.DueDate),
        // The existing Frequency enum defines None, Once, Weekly, Monthly, Quarterly,
        // Seasonally, and Annually in this order. No recurrence behavior is introduced.
        WorkshopTaskSort.RepeatFrequency => tasks
            .OrderBy(task => task.Repeat)
            .ThenBy(task => task.DueDate),
        _ => tasks.OrderBy(task => task.DueDate)
      };
      return sorted.ThenBy(task => task.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }
  }
}
