using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI.Services;

// Creates one login-time reminder for overdue and upcoming maintenance tasks.
internal static class MaintenanceReminderService
{
    // Today through the next seven days, inclusive.
    internal static List<TaskRecord> DueWithinSevenDays(IEnumerable<TaskRecord> tasks)
    {
        DateTime today = DateTime.Today;
        DateTime cutoff = today.AddDays(7);

        return tasks
            .Where(task => IsActive(task)
                && task.DueDate.Date >= today
                && task.DueDate.Date <= cutoff)
            .OrderBy(task => task.DueDate)
            .ToList();
    }

    // Any task with a due date before today that is not complete or archived.
    internal static List<TaskRecord> OverdueTasks(IEnumerable<TaskRecord> tasks)
    {
        DateTime today = DateTime.Today;

        return tasks
            .Where(task => IsActive(task) && task.DueDate.Date < today)
            .OrderBy(task => task.DueDate)
            .ToList();
    }

    // Null means that there are no overdue or upcoming tasks to notify about.
    // Show at most two task names per category so Windows can display the summary.
    internal static string? BuildLoginReminder(IEnumerable<TaskRecord> tasks)
    {
        List<TaskRecord> activeTasks = tasks.Where(IsActive).ToList();
        List<TaskRecord> overdue = OverdueTasks(activeTasks);
        List<TaskRecord> dueSoon = DueWithinSevenDays(activeTasks);

        if (overdue.Count == 0 && dueSoon.Count == 0)
            return null;

        List<string> sections = new();

        if (overdue.Count > 0)
            sections.Add($"Overdue ({overdue.Count}): {DescribeTasks(overdue)}");

        if (dueSoon.Count > 0)
            sections.Add($"Due soon ({dueSoon.Count}): {DescribeTasks(dueSoon)}");

        return string.Join("\n", sections);
    }

    private static bool IsActive(TaskRecord task) =>
        !task.IsArchived && task.Status == Status.Pending;

    private static string DescribeTasks(IReadOnlyList<TaskRecord> tasks)
    {
        string preview = string.Join(", ", tasks.Take(2).Select(task =>
            $"{task.Name} ({task.DueDate:MMM d})"));

        return tasks.Count > 2
            ? $"{preview}, +{tasks.Count - 2} more"
            : preview;
    }
}
