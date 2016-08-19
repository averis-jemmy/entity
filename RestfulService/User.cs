using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAveris
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public Guid UserID { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsRecruiter { get; set; }
        public Guid TokenID { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
    }
}
