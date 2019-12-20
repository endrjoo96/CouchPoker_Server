using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Misc
{
    public static class Security
    {
        private static string EncryptionKey = "AXkQDhrsPyrMqmvEMu3bZd4HISRCJ8KG";

        //code from: 
        public static string Encrypt(string data)
        {
            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                provider.KeySize = 256;
                provider.BlockSize = 128;
                byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);
                provider.Key = key;
                provider.Padding = PaddingMode.PKCS7;
                provider.Mode = CipherMode.ECB;
                ICryptoTransform encrypter = provider.CreateEncryptor();
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] encryptedBytes = encrypter.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public static string Decrypt(string data)
        {
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                csp.KeySize = 256;
                csp.BlockSize = 128;
                csp.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform decrypter = csp.CreateDecryptor();
                byte[] dataBytes = Convert.FromBase64String(data);
                byte[] decryptedBytes = decrypter.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
