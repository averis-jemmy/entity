using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESuntikanAppData
{
    public class BusinessTypeRepository : RepositoryBase<BusinessType>
    {
        public BusinessTypeRepository(ESuntikanLocalDB context) : base(context) { }

        public KeyValuePair<Guid, string>[] GetBusinessTypes()
        {
            var query = from item in GetAll().ToList()
                        where item.IsDeleted == false
                        orderby item.Name
                        select new KeyValuePair<Guid, string>(item.ID, item.Name);

            return query.ToArray();
        }
    }
}
