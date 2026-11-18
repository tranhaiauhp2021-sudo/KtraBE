using System;
using System.Security.Cryptography;

namespace KtraBE.Models
{
    public static class PasswordHasher
    {
        // Lưu mật khẩu dưới dạng hash PBKDF2; không lưu mật khẩu thô.
        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var kdf = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                return "PBKDF2$100000$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(kdf.GetBytes(32));
        }
        // Format: PBKDF2$100000$base64(salt)$base64(hash)
        public static bool Verify(string password, string stored)
        {
            if (String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(stored)) return false;
            try
            {
                var parts = stored.Split('$');
                if (parts.Length != 4 || parts[0] != "PBKDF2") return false;
                int rounds = Int32.Parse(parts[1]);
                if (rounds < 100000) return false;
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                using (var kdf = new Rfc2898DeriveBytes(password, salt, rounds, HashAlgorithmName.SHA256))
                {
                    byte[] actual = kdf.GetBytes(expected.Length);
                    int mismatch = 0;
                    for (int i = 0; i < actual.Length; i++) mismatch |= actual[i] ^ expected[i];
                    return mismatch == 0;
                }
            }
            catch (FormatException) { return false; }
            catch (ArgumentException) { return false; }
        }
    }
}
