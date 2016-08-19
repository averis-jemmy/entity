using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAveris
{
    [Table("DeclarationDetail")]
    public class DeclarationDetail
    {
        [PrimaryKey, Column("ID")]
        public Guid ID { get; set; }
        public byte No { get; set; }
        public string Declaration { get; set; }
    }
}
