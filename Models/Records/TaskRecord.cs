using HomeMaintenanceMAUI.Models;

namespace HomeMaintenanceMAUI.Models.Records;

// Represents a maintenance task in a saved JSON file.
// This record only stores data; task behavior is handled elsewhere.
internal sealed record TaskRecord(
    Guid Id,
    string Name,
    string Description,
    string Type,
    DateTime DueDate,
    Status Status,
    Frequency Repeat = Frequency.None,
    DateTime? ScheduleAnchor = null,
    int Occurrence = 0,
    Guid? PreviousOccurrenceId = null,
    DateTime? CompletedAt = null,
    bool IsArchived = false);