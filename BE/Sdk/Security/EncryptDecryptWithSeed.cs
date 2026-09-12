using System.Security.Cryptography;
using System.Text;
namespace Ordbox.SDK.Security
{
    public static class EncryptDecryptWithSeed
    {
        private const string DefaultKey = "E6t187^D43%F";
        public static RijndaelManaged AES = null;

        private const int Iterations = 10000;

        private static byte[] GenerateSalt()
        {
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        public static byte[] GetPasswordBytes(string? keyOverride = null)
        {
            string key = keyOverride ?? DefaultKey;
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        public static byte[] saltBytes = new byte[8]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8
        };

        public static byte[] AESEncrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, int iterations = Iterations)
        {
            if (AES == null)
            {
                AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Mode = CipherMode.CBC;
            }

            byte[] array = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations);
                AES.Key = rfc2898DeriveBytes.GetBytes(AES.KeySize / 8);
                AES.IV = rfc2898DeriveBytes.GetBytes(AES.BlockSize / 8);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cryptoStream.Close();
                }

                return memoryStream.ToArray();
            }
        }

        public static byte[] AESDecrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, int iterations = Iterations)
        {
            if (AES == null)
            {
                AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Mode = CipherMode.CBC;
            }

            byte[] array = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations);
                AES.Key = rfc2898DeriveBytes.GetBytes(AES.KeySize / 8);
                AES.IV = rfc2898DeriveBytes.GetBytes(AES.BlockSize / 8);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cryptoStream.Close();
                }

                return memoryStream.ToArray();
            }
        }

        public static string EncryptText(string input, string password = "E6t187^D43%F", int iterations = 10)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] bytes2 = Encoding.UTF8.GetBytes(password);
            bytes2 = SHA256.Create().ComputeHash(bytes2);
            return Convert.ToBase64String(AESEncrypt(bytes, bytes2, iterations));
        }

        public static string DecryptText(string input, string password = "E6t187^D43%F", int iterations = 10)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            bytes = SHA256.Create().ComputeHash(bytes);
            byte[] bytes2 = AESDecrypt(bytesToBeDecrypted, bytes, iterations);
            return Encoding.UTF8.GetString(bytes2);
        }
    }
}
