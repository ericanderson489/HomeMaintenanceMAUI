using System.Security.Cryptography;

namespace HomeMaintenanceMAUI.Services;

// Represents the information needed to verify a password.
// The original password is never stored.
internal sealed record PasswordRecord(
    string Salt,
    string Hash,
    int Iterations);

internal static class PasswordSecurity
{   // The number of iterations for the PBKDF2 algorithm.
    private const int Iterations = 600_000;
    // Creates a salted password hash for a new account.
    internal static PasswordRecord Hash(string password)
    {   // Prevent empty passwords from being hashed.
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        // Generate a random 16-byte salt.
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        // Derive a 32-byte hash using PBKDF2 with SHA-256.
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            32);
        // Return a PasswordRecord containing the salt, hash, and iteration count.
        return new PasswordRecord(
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash),
            Iterations);
    }
    // Checks whether a stored password record is valid.
    internal static bool IsValid(PasswordRecord? record)
    {   // A valid record must not be null, must have a reasonable iteration count
        if (record is null ||
            record.Iterations < 600_000 ||
            record.Iterations > 2_000_000)
        {
            return false;
        }

        try
        {   // The salt must be 16 bytes and the hash must be 32 bytes.
            return Convert.FromBase64String(record.Salt).Length == 16
                && Convert.FromBase64String(record.Hash).Length == 32;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentNullException)
        {
            return false;
        }
    }

    // Verifies a login password against the stored hash.
    internal static bool Verify(string password, PasswordRecord record)
    {   // If the record is invalid or the password is null, return false.
        if (!IsValid(record) || password is null) return false;
        // Convert the stored salt and hash from Base64 strings to byte arrays.
        // The reason we use Base64 is to make the salt and hash safe for JSON serialization.
        byte[] salt = Convert.FromBase64String(record.Salt);
        byte[] expectedHash = Convert.FromBase64String(record.Hash);
        // Derive the hash of the provided password using the same salt and iteration count.
        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            record.Iterations,
            HashAlgorithmName.SHA256,
            32);
        // Use a constant-time comparison to prevent timing attacks.
        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash);
    }
}
