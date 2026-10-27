using System;
using System.Security.Cryptography;

namespace nv3.Helpers
{
    /// <summary>Băm mật khẩu bằng PBKDF2 (không bao giờ lưu mật khẩu thô vào database).</summary>
    public static class PasswordHelper
    {
        private const int Iterations = 10000;

        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
            {
                return false;
            }

            string[] parts = stored.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expected = Convert.FromBase64String(parts[1]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] actual = pbkdf2.GetBytes(expected.Length);

                int diff = 0;
                for (int i = 0; i < expected.Length; i++)
                {
                    diff |= expected[i] ^ actual[i];
                }
                return diff == 0;
            }
        }
    }
}
