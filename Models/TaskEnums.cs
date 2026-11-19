namespace HomeMaintenanceMAUI.Models;
// This is still a work in progress. 
// Represents whether a maintenance task is finished.
public enum Status
{
    Pending,
    Complete
}

// Represents how often a maintenance task repeats.
public enum Frequency {
  None,
  Once,
  Weekly,
  Monthly,
  Quarterly,
  Seasonally,
  Annually
}

// Category for tasks
public enum Category {
  Landscaping,
  Appliances,
  Fixtures,
  Safety,
  Security,
  Cleaning,
  Other
}