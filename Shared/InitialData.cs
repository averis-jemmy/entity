using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESuntikanAppData
{
    public class InitialData
    {
        public static bool IsFirstTimeSetup()
        {
            try
            {
                ESuntikanLocalDB db = Database.Instant;

                return db.Vaccines.Count() == 0;
            }
            catch { }

            return true;
        }

        public static KeyValuePair<string, string>[] Genders
        {
            get
            {
                return GetGenders();
            }
        }

        private static KeyValuePair<string, string>[] GetGenders()
        {
            var results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("LELAKI", "LELAKI"));
            results.Add(new KeyValuePair<string, string>("PEREMPUAN", "PEREMPUAN"));

            return results.ToArray();
        }

        public static KeyValuePair<Guid, string>[] Countries
        {
            get
            {
                return GetCountries();
            }
        }

        private static KeyValuePair<Guid, string>[] GetCountries()
        {
            var countryRep = new CountryRepository(Database.Instant);
            return countryRep.GetCountries();
        }

        public static KeyValuePair<Guid, string>[] Races
        {
            get
            {
                return GetRaces();
            }
        }

        private static KeyValuePair<Guid, string>[] GetRaces()
        {
            var raceRep = new RaceRepository(Database.Instant);
            return raceRep.GetRaces();
        }

        public static KeyValuePair<Guid, string>[] Vaccines
        {
            get
            {
                return GetVaccines();
            }
        }

        private static KeyValuePair<Guid, string>[] GetVaccines()
        {
            var vacRep = new VaccineRepository(Database.Instant);
            return vacRep.GetVaccines();
        }

        public static KeyValuePair<Guid, string>[] BusinessTypes
        {
            get
            {
                return GetBusinessTypes();
            }
        }

        private static KeyValuePair<Guid, string>[] GetBusinessTypes()
        {
            var busTypeRep = new BusinessTypeRepository(Database.Instant);
            return busTypeRep.GetBusinessTypes();
        }
    }
}
