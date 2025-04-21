using BCrypt.Net;

namespace RETRA_HotelSys.Utilities
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generate a salt and hash the password
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the password against the stored hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}