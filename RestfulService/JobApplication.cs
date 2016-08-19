using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAveris
{
    [Table("JobApplication")]
    public class JobApplication
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        public byte[] Photo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PositionApplied { get; set; }
        public string KnownAs { get; set; }
        public string ChineseCharacter { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Race { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string IdentityNo { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime? DateOfExpiry { get; set; }
        public string CountryOfIssue { get; set; }
        public string EmailAddress { get; set; }
        public string EmergencyContact { get; set; }
        public string ServiceCompleted { get; set; }
        public string HighestRankAttained { get; set; }
        public string ExemptionReason { get; set; }
        public string LiabilityForDuties { get; set; }
        public string ApplicationStatus { get; set; }
        public bool IsLocked { get; set; }
    }
}
