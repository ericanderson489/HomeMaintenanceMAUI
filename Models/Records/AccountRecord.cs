using HomeMaintenanceMAUI.Services;

namespace HomeMaintenanceMAUI.Models.Records;

// Represents the information saved to a JSON file.
// Unlike Account, this class is designed for serialization.
// The schema and revision fields allow for future changes to the file format.
internal sealed record AccountRecord(
    int Schema,
    int Revision,
    string UserName,
    string FirstName,
    string LastName,
    PasswordRecord Password,
    List<TaskRecord> Tasks);