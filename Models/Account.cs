// We use PasswordSecurity and PasswordRecord from Services and AccountRecord from Models/Records.
using HomeMaintenanceMAUI.Services;
using HomeMaintenanceMAUI.Models.Records;

namespace HomeMaintenanceMAUI.Models;
// Represents a user account.
// Stores basic account information and provides password verification.
// Saving and loading accounts is handled separately by AccountStore.
public class Account {
  // Account information cannot be changed after construction.
  private readonly string m_firstName;
  private readonly string m_lastName;
  private readonly string m_userName;

  // Stores the password hash and salt, not the actual password.
  // This is a record type, so it is immutable and cannot be changed after construction.
  // You can find PasswordRecord in Services/PasswordSecurity.cs.
  private readonly PasswordRecord m_password;

  // Create a list of tasks within the account to store user tasks
  private List<Tasks> m_userTasks;

    // Creates a new account using information entered during registration.
    public Account(string username, string password,
                   string firstName, string lastName)
    {
        // Prevent required fields from being empty.
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        m_userName = username.Trim();
        m_firstName = firstName.Trim();
        m_lastName = lastName.Trim();

        // Convert the password into a salted hash.
        // PasswordSecurity.Hash() is defined in Services/PasswordSecurity.cs.
        m_password = PasswordSecurity.Hash(password);
        
        // Initialize task list
        m_userTasks = new List<Tasks>();
    }
    // Reconstructs an existing account from saved information.
    // Unlike the public constructor, this does not hash the password again.
    // AccountRecord is defined in Models/Records/AccountRecord.cs.
    private Account(AccountRecord record)
    {
        m_userName = record.UserName;
        m_firstName = record.FirstName;
        m_lastName = record.LastName;

        // Reuse the previously saved password hash and salt.
        m_password = record.Password;

        // Initialize task list
        m_userTasks = new List<Tasks>();
  }
    // Creates a record containing the account information needed for storage.
    // This does not write anything to disk.
    internal AccountRecord Snapshot()
    {
        return new AccountRecord(
            Schema: 3,
            Revision: 0,
            UserName: m_userName,
            FirstName: m_firstName,
            LastName: m_lastName,
            Password: m_password,
            Tasks: new List<TaskRecord>()
        );
    }
    // Creates an Account object from a previously saved record.
    internal static Account Restore(AccountRecord record)
    {
        return new Account(record);
    }
    // Return account information when requested.
    public string GetAccountFullName()
    {
        return m_firstName + " " + m_lastName;
    }

    public string GetAccountFirstName()
    {
        return m_firstName;
    }

    public string GetAccountLastName()
    {
        return m_lastName;
    }

    public string GetAccountUserName()
    {
        return m_userName;
    }

    // Get and Set for m_userTasks
    public void SetUserTasks(Tasks task) 
    {
      m_userTasks.Add(task);
    }

    public List<Tasks> GetUserTasks() 
    {
      return m_userTasks;
    }

    // Checks whether a supplied password matches the stored hash.
    public bool VerifyPassword(string candidate)
    {
        return PasswordSecurity.Verify(candidate, m_password);
    }

    // Allows an account to be displayed by its full name.
    public override string ToString()
    {
        return GetAccountFullName();
    }
}