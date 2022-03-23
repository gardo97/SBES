using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SymetricAlgorithmAES
{
    public class AESinCBC
    {
        /*
        public static byte[] Encryption(string message, byte[] secretKey, byte[] IV)
        {
            if(message == null || message.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (secretKey == null || secretKey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = secretKey;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(message);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
                return encrypted;
            }/*
                //AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
                //{
                  //  Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                    //Mode = CipherMode.ECB,
                    //Padding = PaddingMode.PKCS7
                //};

            ICryptoTransform encryptTransform = aesCryptoProvider.CreateEncryptor();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(AlarmInByteArr, 0, AlarmInByteArr.Length);
                    cryptoStream.FlushFinalBlock();
                    encryptedAlarm = memoryStream.ToArray();
                }
            }

            return encryptedAlarm;*/
        // }
        /*
            public static string Decryption(byte[] cipherText, byte[] Key, byte[] IV)
            {
                // Check arguments.
                if (cipherText == null || cipherText.Length <= 0)
                    throw new ArgumentNullException("cipherText");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");

                // Declare the string used to hold
                // the decrypted text.
                string plaintext = null;

                // Create an Aes object
                // with the specified key and IV.
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;
                    aesAlg.Mode = CipherMode.CBC;
                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                           // using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                          //  {
                               plaintext = csDecrypt.Read()
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                             //   plaintext = srDecrypt.ReadToEnd();


                          ///  }
                        }
                    }
                }

                return plaintext;
            }
            */

        public static byte[] Decrypt(byte[] inFile, string secretKey, CipherMode mode)
        {
            byte[] header = null;       //image header (54 byte) should not be decrypted
            byte[] body = null;         //image body to be decrypted
            byte[] decryptedBody = null;

            FormatterKey.Decompose(inFile, out header, out body);
            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = mode,
                Padding = PaddingMode.None
            };

            aesCryptoProvider.IV = body.Take(aesCryptoProvider.BlockSize / 8).ToArray();                // take the iv off the beginning of the ciphertext message			
            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();

            using (MemoryStream memoryStream = new MemoryStream(body.Skip(aesCryptoProvider.BlockSize / 8).ToArray()))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedBody = new byte[body.Length - aesCryptoProvider.BlockSize / 8];     //decrypted image body - the same lenght as encrypted part
                    cryptoStream.Read(decryptedBody, 0, decryptedBody.Length);
                }
            }
            
            int outputLenght = header.Length + decryptedBody.Length;
           return FormatterKey.Compose(header, decryptedBody, outputLenght);
        }
        public static byte[] Encrypt(byte[] inFile, string secretKey, CipherMode mode)
        {
            byte[] header = null;   //image header (54 byte) should not be encrypted
            byte[] body = null;     //image body to be encrypted
            byte[] encryptedBody = null;

            FormatterKey.Decompose(inFile, out header, out body);

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = mode,
                Padding = PaddingMode.PKCS7
            };

            aesCryptoProvider.GenerateIV();

            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(body, 0, body.Length);
                    encryptedBody = aesCryptoProvider.IV.Concat(memoryStream.ToArray()).ToArray();    //encrypted image body with IV	
                }
                
            }
            
            int outputLenght = header.Length + encryptedBody.Length;              //header.Length + body.Length
            return FormatterKey.Compose(header, encryptedBody, outputLenght);

        }


    }
}
