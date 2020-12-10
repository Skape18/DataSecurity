using Lab5_6_7.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5_6_7.Services
{
    public interface ISensitiveDataService
    {
        void EncryptAndSave(string userId, SensitiveDataDto sensitiveData);

        SensitiveDataDto RetrieveAndDecrypt(string userId);
    }
}
