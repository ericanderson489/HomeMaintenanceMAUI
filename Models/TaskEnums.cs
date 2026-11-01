namespace HomeMaintenanceMAUI.Models;
// This is still a work in progress. 
// Represents whether a maintenance task is finished.
internal enum Status
{
    Pending,
    Complete
}

// Represents how often a maintenance task repeats.
public enum RepeatSchedule
{
    None,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}