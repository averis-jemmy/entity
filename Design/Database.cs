using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESuntikanAppData
{
    public class Database
    {
        public static ESuntikanLocalDB Instant
        {
            get
            {
                return new ESuntikanLocalDB();
            }
        }

    }
}
