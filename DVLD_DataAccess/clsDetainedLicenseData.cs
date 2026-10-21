using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsDetainedLicenseData
    {
        public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID, ref int DetainID, ref DateTime DetainDate, ref float FineFees,
            ref int CreatedByUserID, ref DateTime ReleaseDate, ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 *
                                     FROM DetainedLicenses
                                     WHERE LicenseID = @LicenseID
                                     AND IsReleased = 0
                                     ORDER BY DetainID DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                DetainID = Convert.ToInt32(Reader["DetainID"]);
                                DetainDate = Convert.ToDateTime(Reader["DetainDate"]);
                                FineFees = Convert.ToSingle(Reader["FineFees"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);

                                if (Reader["ReleaseDate"] != DBNull.Value)
                                {
                                    ReleaseDate = Convert.ToDateTime(Reader["ReleaseDate"]);
                                }
                                else
                                    ReleaseDate = DateTime.MinValue;

                                if (Reader["ReleasedByUserID"] != DBNull.Value)
                                {
                                    ReleasedByUserID = Convert.ToInt32(Reader["ReleasedByUserID"]);
                                }
                                else
                                    ReleasedByUserID = -1;

                                if (Reader["ReleaseApplicationID"] != DBNull.Value)
                                {
                                    ReleaseApplicationID = Convert.ToInt32(Reader["ReleaseApplicationID"]);
                                }
                                else
                                    ReleaseApplicationID = -1;
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsFound;
        }

        public static bool GetDetainedLicenseInfoByDetainID(int DetainID, ref int LicenseID, ref DateTime DetainDate, ref float FineFees,
            ref int CreatedByUserID, ref bool IsReleased, ref DateTime ReleaseDate, ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM DetainedLicenses
                                     WHERE DetainID = @DetainID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DetainID", DetainID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                LicenseID = Convert.ToInt32(Reader["LicenseID"]);
                                DetainDate = Convert.ToDateTime(Reader["DetainDate"]);
                                FineFees = Convert.ToSingle(Reader["FineFees"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                                IsReleased = Convert.ToBoolean(Reader["IsReleased"]);

                                if (Reader["ReleaseDate"] != DBNull.Value)
                                {
                                    ReleaseDate = Convert.ToDateTime(Reader["ReleaseDate"]);
                                }
                                else
                                    ReleaseDate = DateTime.MinValue;

                                if (Reader["ReleasedByUserID"] != DBNull.Value)
                                {
                                    ReleasedByUserID = Convert.ToInt32(Reader["ReleasedByUserID"]);
                                }
                                else
                                    ReleasedByUserID = -1;

                                if (Reader["ReleaseApplicationID"] != DBNull.Value)
                                {
                                    ReleaseApplicationID = Convert.ToInt32(Reader["ReleaseApplicationID"]);
                                }
                                else
                                    ReleaseApplicationID = -1;
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsFound;
        }

        public static int AddNewDetainedLicense(int LicenseID, DateTime DetainDate, float FineFees,
            int CreatedByUserID, bool IsReleased, DateTime ReleaseDate, int ReleasedByUserID, int ReleaseApplicationID)
        {
            int DetainID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO DetainedLicenses
                                     (LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased, ReleaseDate, ReleasedByUserID, ReleaseApplicationID)
                                     VALUES
                                     (@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, @IsReleased, @ReleaseDate, @ReleasedByUserID, @ReleaseApplicationID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        Command.Parameters.AddWithValue("@DetainDate", DetainDate);
                        Command.Parameters.AddWithValue("@FineFees", FineFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@IsReleased", IsReleased);

                        if (ReleaseDate != DateTime.MinValue)
                            Command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
                        else
                            Command.Parameters.AddWithValue("@ReleaseDate", DBNull.Value);

                        if (ReleasedByUserID != -1)
                            Command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                        else
                            Command.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);

                        if (ReleaseApplicationID != -1)
                            Command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
                        else
                            Command.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);

                        Connection.Open();

                        DetainID = Convert.ToInt32(Command.ExecuteScalar());
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return DetainID;
        }

        public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, float FineFees,
            int CreatedByUserID, bool IsReleased, DateTime ReleaseDate, int ReleasedByUserID, int ReleaseApplicationID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE DetainedLicenses
                                     SET LicenseID = @LicenseID,
	                                     DetainDate = @DetainDate,
	                                     FineFees = @FineFees,
	                                     CreatedByUserID = @CreatedByUserID,
	                                     IsReleased = @IsReleased,
	                                     ReleaseDate = @ReleaseDate,
	                                     ReleasedByUserID = @ReleasedByUserID,
	                                     ReleaseApplicationID = @ReleaseApplicationID
                                     WHERE DetainID = @DetainID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DetainID", DetainID);
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        Command.Parameters.AddWithValue("@DetainDate", DetainDate);
                        Command.Parameters.AddWithValue("@FineFees", FineFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@IsReleased", IsReleased);

                        if (ReleaseDate != DateTime.MinValue)
                            Command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
                        else
                            Command.Parameters.AddWithValue("@ReleaseDate", DBNull.Value);

                        if (ReleasedByUserID != -1)
                            Command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                        else
                            Command.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);

                        if (ReleaseApplicationID != -1)
                            Command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
                        else
                            Command.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }

        public static bool ReleaseDetainedLicense(int DetainID, int ReleasedByUserID, int ReleaseApplicationID)
        {
            int RowsAffected = 0;
            
            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE DetainedLicenses
                                     SET ReleaseDate = @ReleaseDate,
	                                     ReleasedByUserID = @ReleasedByUserID,
	                                     ReleaseApplicationID = @ReleaseApplicationID,
                                         IsReleased = 1
                                     WHERE DetainID = @DetainID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DetainID", DetainID);
                        Command.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);
                        Command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                        Command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }

        public static DataTable GetAllDetainedLicenses()
        {
            DataTable DetainedLicenses = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT DetainID AS [Detain ID], DetainedLicenses.LicenseID AS [License ID], DetainDate AS [Detain Date], IsReleased AS [Is Released?], FineFees AS [Fine Fees],
                                     ReleaseDate AS [Release Date], People.NationalNo AS [National No.],
                                     CONCAT_WS(' ', People.FirstName, People.SecondName, People.ThirdName, People.LastName) AS [Full Name],
                                     ReleaseApplicationID AS [Release Application ID]
                                     FROM DetainedLicenses
                                     INNER JOIN Licenses
                                     ON DetainedLicenses.LicenseID = Licenses.LicenseID
                                     INNER JOIN Drivers
                                     ON Licenses.DriverID = Drivers.DriverID
                                     INNER JOIN People
                                     ON Drivers.PersonID = People.PersonID
                                     ORDER BY DetainID DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                DetainedLicenses.Load(Reader);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return DetainedLicenses;
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            bool IsDetained = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM DetainedLicenses
                                     WHERE LicenseID = @LicenseID
                                     AND IsReleased = 0";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null)
                            IsDetained = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsDetained;
        }
    }
}