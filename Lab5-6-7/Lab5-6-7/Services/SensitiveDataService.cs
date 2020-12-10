using Lab5_6_7.Models.DTOs;
using System;
using System.Buffers.Binary;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Lab5_6_7.Data;
using Lab5_6_7.Models.Entities;

namespace Lab5_6_7.Services
{
    public class SensitiveDataService : ISensitiveDataService
    {
        private byte[] _key;
        private ApplicationDbContext _context;

        public SensitiveDataService(ApplicationDbContext context, IEncryptionKeyProvider provider)
        {
            _context = context;
            _key = Convert.FromBase64String(provider.GetKey());
        }
        
        public void EncryptAndSave(string userId, SensitiveDataDto sensitiveData)
        {
            var dbSensitiveData = _context.SensitiveData.FirstOrDefault(d => d.UserId == userId);

            if (dbSensitiveData == null)
            {
                _context.Add(new SensitiveData
                {
                    UserId = userId,
                    EncryptedSensitiveData = Encrypt(sensitiveData.SensitiveData)
                });
            }
            else
            {
                dbSensitiveData.EncryptedSensitiveData = Encrypt(sensitiveData.SensitiveData);
            }

            _context.SaveChanges();
        }

        public SensitiveDataDto RetrieveAndDecrypt(string userId)
        {
            var dbSensitiveData = _context.SensitiveData.FirstOrDefault(d => d.UserId == userId);

            var sensitiveData = new SensitiveDataDto();

            if (dbSensitiveData != null)
                sensitiveData.SensitiveData = Decrypt(dbSensitiveData.EncryptedSensitiveData);

            return sensitiveData;
        }

        private string Encrypt(string plain)
        {
            // Get bytes of plaintext string
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

            // Get parameter sizes
            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            // We write everything into one big array for easier encoding
            int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < 1024
                ? stackalloc byte[encryptedDataLength]
                : new byte[encryptedDataLength].AsSpan();

            // Copy parameters
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Generate secure nonce
            RandomNumberGenerator.Fill(nonce);

            // Encrypt
            using var aes = new AesGcm(_key);
            aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            // Encode for transmission
            return Convert.ToBase64String(encryptedData);
        }

        public string Decrypt(string cipher)
        {
            // Decode
            Span<byte> encryptedData = Convert.FromBase64String(cipher).AsSpan();

            // Extract parameter sizes
            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            // Extract parameters
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Decrypt
            Span<byte> plainBytes = cipherSize < 1024
                ? stackalloc byte[cipherSize]
                : new byte[cipherSize];
            using var aes = new AesGcm(_key);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            // Convert plain bytes back into string
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
