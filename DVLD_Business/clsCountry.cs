using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_DataAccess;
using System.Data;

namespace DVLD_Business
{
    public class clsCountry
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public clsCountry()
        {
            ID = -1;
            Name = "";
        }

        /* Access modifier (private) to prevent dev. to instantiate an object from this constructor
        except by using Find method to check whether country presents in database or not */
        private clsCountry(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }

        public static clsCountry Find(int CountryID)
        {
            string CountryName = "";

            if (clsCountryData.GetCountryInfoByID(CountryID, ref CountryName))
            {
                return new clsCountry(CountryID, CountryName);
            }

            return null;
        }

        public static clsCountry Find(string CountryName)
        {
            int CountryID = -1;

            if (clsCountryData.GetCountryInfoByName(CountryName, ref CountryID))
            {
                return new clsCountry(CountryID, CountryName);
            }

            return null;
        }

        public static DataTable GetAllCountries()
        {
            return clsCountryData.GetAllCountries();
        }
    }
}