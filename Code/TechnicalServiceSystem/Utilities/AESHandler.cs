using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TechnicalServiceSystem.Utilities
{
    public static class AESHandler
    {
        private const string AES_Key = "d191f0b58b2d2d99a9976d32925b1dba92274b7c4e216d034258649286c51b68";
        private const string AES_IV = "50358926fb47aa6ae8cdeafc0daf476a";

        public static string EncryptString(string input)
        {
            string output = null;
            using (Aes aes = Aes.Create())
            {
                var key = new byte[AES_Key.Length / 2];
                var iv = new byte[AES_IV.Length / 2];

                for (var i = 0; i < key.Length; i++)
                    key[i] = Convert.ToByte(AES_Key.Substring(i * 2, 2), 16);

                for (var i = 0; i < iv.Length; i++)
                    iv[i] = Convert.ToByte(AES_IV.Substring(i * 2, 2), 16);

                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    byte[] encrypted = null;
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(input);
                        }
                        encrypted = msEncrypt.ToArray();
                    }

                    StringBuilder hex = new StringBuilder(encrypted.Length * 2);
                    foreach (byte b in encrypted)
                        hex.AppendFormat("{0:x2}", b);

                    output = hex.ToString();
                }
            }
            return output;
        }

        public static string DecryptString(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || (input.Length % 2 != 0))
                return null;

            string output = null;
            using (Aes aes = Aes.Create())
            {
                var key = new byte[AES_Key.Length / 2];
                var iv = new byte[AES_IV.Length / 2];

                for (var i = 0; i < key.Length; i++)
                    key[i] = Convert.ToByte(AES_Key.Substring(i * 2, 2), 16);

                for (var i = 0; i < iv.Length; i++)
                    iv[i] = Convert.ToByte(AES_IV.Substring(i * 2, 2), 16);

                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                var inputbytes = new byte[input.Length / 2];

                for (var i = 0; i < inputbytes.Length; i++)
                    inputbytes[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(inputbytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            output = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return output;
        }
    }
}
