using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESuntikanAppData
{
    public class RaceRepository : RepositoryBase<Race>
    {
        public RaceRepository(ESuntikanLocalDB context) : base(context) { }

        public KeyValuePair<Guid, string>[] GetRaces()
        {
            var query = from item in GetAll().ToList()
                        where item.IsDeleted == false
                        orderby item.Name
                        select new KeyValuePair<Guid, string>(item.ID, item.Name);

            return query.ToArray();
        }
    }
}
