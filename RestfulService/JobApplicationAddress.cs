using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAveris
{
    [Table("JobApplicationAddress")]
    public class JobApplicationAddress
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public string AddressType { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomeNumber { get; set; }
        public string MobileNumber { get; set; }
    }
}
