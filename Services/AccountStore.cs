using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HomeMaintenanceMAUI.Models;
using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI.Services;
// Saves and loads user accounts as JSON files.
// Each account is stored in its own file.
internal sealed class AccountStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
    // The directory where account files are stored.
    private readonly string directoryPath;
    // Creates a new account store.
    public AccountStore()
    {
        // MAUI provides a writable, app-specific storage directory.
        directoryPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "Accounts");
    }
    // Creates a filename from the username.
    // The SHA-256 hash avoids unsafe filename characters.
    // This is NOT password hashing.
    private string GetAccountPath(string username)
    {   // Normalize the username to avoid case sensitivity and whitespace issues.
        string normalized = username.Trim().ToUpperInvariant();
        // Hash the normalized username using SHA-256.
        // We do this because usernames may contain characters that are not safe for filenames.
        byte[] bytes = Encoding.UTF8.GetBytes(normalized);
        byte[] hash = SHA256.HashData(bytes);
        // Convert the hash to a hexadecimal string and append the .json extension.
        string filename = Convert.ToHexString(hash) + ".json";
        // Combine the directory path and filename to get the full path.
        return Path.Combine(directoryPath, filename);
    }
    // Loads all previously saved accounts.
    public List<Account> LoadAccounts()
    {   // accounts is the list of accounts that will be returned. It starts empty.
        List<Account> accounts = new();
        // If the directory does not exist, return an empty list.
        if (!Directory.Exists(directoryPath))
            return accounts;
        // Enumerate all JSON files in the directory.
        foreach (string path in Directory.EnumerateFiles(directoryPath, "*.json"))
        {   // Read the account record from the file.
            AccountRecord record = ReadAccount(path);
            // Check that the filename matches the stored username.
            if (!string.Equals(
                Path.GetFullPath(path),
                Path.GetFullPath(GetAccountPath(record.UserName)),
                StringComparison.OrdinalIgnoreCase))
            {   // The filename does not match the username.
                throw new InvalidDataException(
                    "An account file has an unexpected filename.");
            }
            // Restore the account from the record and add it to the list.
            accounts.Add(Account.Restore(record));
        }
        // Return the list of accounts.
        return accounts;
    }
    // Loads the maintenance tasks for a specific account. Login uses this
    // because LoadAccounts() creates Account objects without exposing Tasks.
    public List<TaskRecord> LoadTasks(string username)
    {
        string path = GetAccountPath(username);
        return ReadAccount(path).Tasks.ToList();
    }
    // Method to save a task. Method added 11/11/26
    public void AddTask(string username, TaskRecord task) {
    string path = GetAccountPath(username);
    // Hold the lock from read through replacement so two app instances
    // can't read the same revision and overwrite each other's changes.
    using FileStream writeLock = new(
        path + ".lock", FileMode.OpenOrCreate,
        FileAccess.Write, FileShare.None);

    AccountRecord account = ReadAccount(path);
    // Preserve the account details and existing tasks while appending
    // the new task and advancing the revision.
    AccountRecord updated = account with {
      Revision = account.Revision + 1,
      Tasks = [.. account.Tasks, task]
    };
    // Write the updated JSON to a temporary file first. If writing fails,
    // the existing account file is still intact.
    string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
    try {
      using (FileStream stream = new(
          tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
        JsonSerializer.Serialize(stream, updated, JsonOptions);
        stream.Flush(true);
      }

      File.Move(tempPath, path, overwrite: true);
    } finally {
      File.Delete(tempPath);
    }
  }
  /// <summary>
  /// Replaces one unchanged task by ID while retaining its position and lifecycle metadata.
  /// </summary>
  /// <param name="username">The account that owns the task.</param>
  /// <param name="original">The snapshot shown when the editor opened.</param>
  /// <param name="edited">The five editable values submitted by the form.</param>
  /// <returns>The record actually written to the account file.</returns>
  /// <exception cref="InvalidOperationException">
  /// The task was removed or changed after the editor opened.
  /// </exception>
  public TaskRecord UpdateTask(string username, TaskRecord original, TaskRecord edited) {
    // BEFORE: No UpdateTask method existed. The only task-save method, AddTask, used:
    // AccountRecord updated = account with {
    //   Revision = account.Revision + 1,
    //   Tasks = [.. account.Tasks, task]
    // };
    // AFTER: AddTask above is unchanged. This new method replaces a matching list slot:
    // tasks[index] = saved;
    // AccountRecord updated = account with {
    //   Revision = account.Revision + 1,
    //   Tasks = tasks
    // };
    // The replacement keeps the same ID, task count, and list order instead of appending.
    if (edited.Id != original.Id) {
      throw new ArgumentException("An edit must keep the original task ID.", nameof(edited));
    }

    string path = GetAccountPath(username);
    // NEW: Use the same exclusive lock as AddTask from read through file replacement.
    using FileStream writeLock = new(
        path + ".lock", FileMode.OpenOrCreate,
        FileAccess.Write, FileShare.None);
    AccountRecord account = ReadAccount(path);
    List<TaskRecord> tasks = account.Tasks.ToList();
    // BEFORE:
    // int index = tasks.FindIndex(task => task.Id == original.Id);
    // AFTER: Reject null stored entries before either ID lookup, using the existing
    // InvalidDataException save-error handler without changing the account file:
    // if (tasks.Any(task => task is null)) {
    //   throw new InvalidDataException("The account contains a null task.");
    // }
    // int index = tasks.FindIndex(task => task.Id == original.Id);
    if (tasks.Any(task => task is null)) {
      throw new InvalidDataException("The account contains a null task.");
    }
    int index = tasks.FindIndex(task => task.Id == original.Id);
    if (index < 0 || tasks[index] != original) {
      throw new InvalidOperationException(
          "This task was changed or removed. Reopen it from the task list before editing.");
    }
    if (tasks.Count(task => task.Id == original.Id) != 1) {
      throw new InvalidDataException("The account contains duplicate task IDs.");
    }

    // NEW: Copy only form fields onto the stored record. No prior update code existed.
    // Id, Status, ScheduleAnchor, Occurrence, PreviousOccurrenceId, CompletedAt, and
    // IsArchived retain their stored values; recurrence/completion logic is not changed.
    TaskRecord saved = tasks[index] with {
      Name = edited.Name,
      Description = edited.Description,
      Type = edited.Type,
      DueDate = edited.DueDate,
      Repeat = edited.Repeat
    };
    tasks[index] = saved;
    // BEFORE:
    // AccountRecord updated = account with {
    //   Revision = account.Revision + 1,
    //   Tasks = tasks
    // };
    // AFTER: Stop this edit before writing a revision that would wrap below zero:
    // if (account.Revision == int.MaxValue) {
    //   throw new InvalidDataException("The account revision cannot be increased.");
    // }
    // AccountRecord updated = account with {
    //   Revision = account.Revision + 1,
    //   Tasks = tasks
    // };
    if (account.Revision == int.MaxValue) {
      throw new InvalidDataException("The account revision cannot be increased.");
    }
    AccountRecord updated = account with {
      Revision = account.Revision + 1,
      Tasks = tasks
    };

    // NEW: Match AddTask's temporary-file write so a failed write preserves the old file.
    string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
    try {
      using (FileStream stream = new(
          tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
        JsonSerializer.Serialize(stream, updated, JsonOptions);
        stream.Flush(true);
      }
      File.Move(tempPath, path, overwrite: true);
    } finally {
      File.Delete(tempPath);
    }
    return saved;
  }

  // Saves a newly registered account.
  // Existing usernames cannot be overwritten.
  public void CreateAccount(Account account)
    {   // Create a record from the account.
        // Snapshot() is defined in Models/Account.cs.
        AccountRecord record = account.Snapshot();
        // Ensure the directory exists.
        Directory.CreateDirectory(directoryPath);
        // Write the record to a new file.
        string path = GetAccountPath(record.UserName);

        // FileMode.CreateNew fails if the account already exists.
        using FileStream stream = new(
            path,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);
        // Save the record with a revision number of 1.
        AccountRecord savedRecord = record with { Revision = 1 };
        // Serialize the record to JSON and write it to the file.
        JsonSerializer.Serialize(stream, savedRecord, JsonOptions);
        stream.Flush(true);
    }
    // Reads one account record from its JSON file.
    private static AccountRecord ReadAccount(string path)
    {
        try
        {   // Open the file for reading.
            using FileStream stream = File.OpenRead(path);
            // Deserialize the JSON into an AccountRecord.
            // This is so we can validate the data before creating an Account object.
            AccountRecord? record = JsonSerializer.Deserialize<AccountRecord>(stream);
            // Validate the record to ensure it has all required fields and correct schema/revision.
            if (record is null ||
                record.Schema != 3 ||
                record.Revision < 1 ||
                string.IsNullOrWhiteSpace(record.UserName) ||
                string.IsNullOrWhiteSpace(record.FirstName) ||
                string.IsNullOrWhiteSpace(record.LastName) ||
                !PasswordSecurity.IsValid(record.Password) ||
                record.Tasks is null)
            {   // The record is invalid or missing required fields.
                throw new InvalidDataException(
                    "An account file contains invalid data.");
            }
            // Return the valid record.
            return record;
        }
        catch (JsonException ex)
        {   // The JSON is malformed or cannot be deserialized.
            throw new InvalidDataException(
                "An account file contains invalid JSON.", ex);
        }
    }
}
