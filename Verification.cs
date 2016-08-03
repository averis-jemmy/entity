using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceEntity
{
    public class Verification
    {
        public string PhoneNumber { get; set; }
        public string VerificationCode { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
    }
}
