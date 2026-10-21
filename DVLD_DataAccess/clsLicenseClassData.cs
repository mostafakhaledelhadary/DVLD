using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsLicenseClassData
    {
        public static DataTable GetAllLicenseClasses()
        {
            DataTable LicenseClasses = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM LicenseClasses";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                LicenseClasses.Load(Reader);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LicenseClasses;
        }

        public static bool GetLicenseClassInfoByID(int LicenseClassID, ref string ClassName,
            ref string ClassDescription, ref byte MinimumAllowedAge, ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool IsFound = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM LicenseClasses
                                     WHERE LicenseClassID = @LicenseClassID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                ClassName = Convert.ToString(Reader["ClassName"]);
                                ClassDescription = Convert.ToString(Reader["ClassDescription"]);
                                MinimumAllowedAge = Convert.ToByte(Reader["MinimumAllowedAge"]);
                                DefaultValidityLength = Convert.ToByte(Reader["DefaultValidityLength"]);
                                ClassFees = Convert.ToSingle(Reader["ClassFees"]);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsFound;
        }

        public static bool GetLicenseClassInfoByClassName(string ClassName, ref int LicenseClassID,
            ref string ClassDescription, ref byte MinimumAllowedAge, ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool IsFound = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM LicenseClasses
                                     WHERE ClassName = @ClassName";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ClassName", ClassName);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                LicenseClassID = Convert.ToInt32(Reader["LicenseClassID"]);
                                ClassDescription = Convert.ToString(Reader["ClassDescription"]);
                                MinimumAllowedAge = Convert.ToByte(Reader["MinimumAllowedAge"]);
                                DefaultValidityLength = Convert.ToByte(Reader["DefaultValidityLength"]);
                                ClassFees = Convert.ToSingle(Reader["ClassFees"]);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsFound;
        }

        public static int AddNewLicenseClass(string ClassName, string ClassDescription, byte MinimumAllowedAge,
            byte DefaultValidityLength, float ClassFees)
        {
            int LicenseClassID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO LicenseClasses
                                     (ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees)
                                     VALUES
                                     (@ClassName, @ClassDescription, @MinimumAllowedAge, @DefaultValidityLength, @ClassFees);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ClassName", ClassName);
                        Command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                        Command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                        Command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                        Command.Parameters.AddWithValue("@ClassFees", ClassFees);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null && int.TryParse(Result.ToString(), out int ID))
                        {
                            LicenseClassID = ID;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LicenseClassID;
        }

        public static bool UpdateLicenseClass(int LicenseClassID, string ClassName, string ClassDescription,
            byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE LicenseClasses
                                     SET ClassName = @ClassName,
	                                     ClassDescription = @ClassDescription,
	                                     MinimumAllowedAge = @MinimumAllowedAge,
										 DefaultValidityLength = @DefaultValidityLength,
										 ClassFees = @ClassFees
                                     WHERE LicenseClassID = @LicenseClassID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                        Command.Parameters.AddWithValue("@ClassName", ClassName);
                        Command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                        Command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                        Command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                        Command.Parameters.AddWithValue("@ClassFees", ClassFees);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }
    }
}