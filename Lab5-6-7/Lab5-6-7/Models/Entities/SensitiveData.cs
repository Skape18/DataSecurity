using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5_6_7.Models.Entities
{
    public class SensitiveData
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EncryptedSensitiveData { get; set; }
    }
}
