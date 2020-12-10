using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;

namespace Lab5_6_7.Services
{
    public class EncryptionKeyProvider : IEncryptionKeyProvider
    {
        private SecretClient _client;

        public EncryptionKeyProvider(SecretClient client)
        {
            _client = client;
        }

        public string GetKey()
        {
            var response = _client.GetSecret("lab-5");

            return response.Value.Value;
        }
    }
}
