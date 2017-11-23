using System.Security.Cryptography;
using System.Text;

namespace YousignClientNETCore
{
    public static class PasswordHelper
    {
        public static string GetPasswordHash(string password)
        {
            return GetHash(GetHash(password) + GetHash(password));
        }

        private static string GetHash(string param)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(param));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
