using System.Security.Cryptography;


namespace Stegosaurus.Crypto
{
    public static class AesCryptoService
    {
        public static void CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be empty!");
        }

        public static byte[] BuildEncryptedData(byte[] salt, byte[] iv, byte[] ciphertext)
        {
            byte[] encryptedData = new byte[salt.Length + iv.Length + ciphertext.Length];
            Buffer.BlockCopy(salt, 0, encryptedData, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, encryptedData, salt.Length, iv.Length);
            Buffer.BlockCopy(ciphertext, 0, encryptedData, salt.Length + iv.Length, ciphertext.Length);

            return encryptedData;
        }
        
        public static byte[] Encrypt(string password, string plaintext)
        {
            CheckPassword(password);
            if (string.IsNullOrEmpty(plaintext)) throw new ArgumentException("Plaintext message cannot be empty!");

            (byte[] key, byte[] salt) = KeyDerivationService.DeriveKeyFromPassword(password);

            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                ICryptoTransform encryptor = aes.CreateEncryptor(key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plaintext);
                        }
                    }
                    byte[] ciphertext = msEncrypt.ToArray();
                    return BuildEncryptedData(salt: salt, iv: aes.IV, ciphertext: ciphertext);
                }
            }
        }

        public static string Decrypt(string password, byte[] encryptedData)
        {
            CheckPassword(password);
            if (encryptedData.Length == 0) throw new ArgumentException("Encrypted data must not be empty!");

            var salt = encryptedData[..16];
            var iv = encryptedData[16..32];
            var ciphertext = encryptedData[32..];
            var key = KeyDerivationService.DeriveKeyFromPassword(password, salt);

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}