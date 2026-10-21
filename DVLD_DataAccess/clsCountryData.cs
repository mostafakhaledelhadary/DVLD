using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsCountryData
    {
        public static bool GetCountryInfoByID(int CountryID, ref string CountryName)
        {
            bool isFound = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Countries
                                     WHERE CountryID = @CountryID;";

                    using(SqlCommand Command =  new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@CountryID", CountryID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                isFound = true;
                                CountryName = (string)Reader["CountryName"];
                            }
                        }
                    }
                }

            }

            catch
            {
                isFound = false;
            }

            return isFound;
        }

        public static bool GetCountryInfoByName(string CountryName, ref int CountryID)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Countries
                                     WHERE CountryName = @CountryName;";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@CountryName", CountryName);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                isFound = true;
                                CountryID = (int)Reader["CountryID"];
                            }
                        }
                    }
                }

            }

            catch
            {
                isFound = false;
            }

            return isFound;
        }

        public static DataTable GetAllCountries()
        {
            DataTable Countries = new DataTable();

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Countries
                                     ORDER BY CountryName";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                Countries.Load(Reader);
                        }
                    }
                }

            }

            catch
            {

            }

            return Countries;
        }
    }
}
