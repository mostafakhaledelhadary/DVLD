using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;


namespace DVLD_DataAccess
{
    public class clsLicenseData
    {
        public static bool GetLicenseInfoByLicenseID(int LicenseID, ref int ApplicationID, ref int DriverID, ref int LicenseClassID,
                  ref DateTime IssueDate, ref DateTime ExpirationDate, ref string Notes, ref float PaidFees,
                  ref bool IsActive, ref byte IssueReason, ref int CreatedByUserID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Licenses
                                     WHERE LicenseID = @LicenseID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                ApplicationID = Convert.ToInt32(Reader["ApplicationID"]);
                                DriverID = Convert.ToInt32(Reader["DriverID"]);
                                LicenseClassID = Convert.ToInt32(Reader["LicenseClassID"]);
                                IssueDate = Convert.ToDateTime(Reader["IssueDate"]);
                                ExpirationDate = Convert.ToDateTime(Reader["ExpirationDate"]);

                                if (Reader["Notes"] != DBNull.Value)
                                {
                                    Notes = (string)Reader["Notes"];
                                }
                                else
                                    Notes = "";

                                PaidFees = Convert.ToSingle(Reader["PaidFees"]);
                                IsActive = Convert.ToBoolean(Reader["IsActive"]);
                                IssueReason = Convert.ToByte(Reader["IssueReason"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
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
        public static int AddNewLicense(int ApplicationID, int DriverID, int LicenseClassID, DateTime IssueDate,
                 DateTime ExpirationDate, string Notes, float PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)
        {
            int LicenseID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO Licenses
                                     (ApplicationID, DriverID, LicenseClassID, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID)
                                     VALUES
                                     (@ApplicationID, @DriverID, @LicenseClassID, @IssueDate, @ExpirationDate, @Notes, @PaidFees, @IsActive, @IssueReason, @CreatedByUserID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@DriverID", DriverID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                        Command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        Command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                        if (!string.IsNullOrEmpty(Notes))
                            Command.Parameters.AddWithValue("@Notes", Notes);
                        else
                            Command.Parameters.AddWithValue("@Notes", DBNull.Value);

                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);
                        Command.Parameters.AddWithValue("@IssueReason", IssueReason);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        Connection.Open();

                        LicenseID = Convert.ToInt32(Command.ExecuteScalar());
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LicenseID;
        }
                 
        public static bool UpdateLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClassID,
                  DateTime IssueDate, DateTime ExpirationDate, string Notes, float PaidFees,
                  bool IsActive, byte IssueReason, int CreatedByUserID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Licenses
                                     SET ApplicationID = @ApplicationID,
	                                     DriverID = @DriverID,
	                                     LicenseClassID = @LicenseClassID,
	                                     IssueDate = @IssueDate,
	                                     ExpirationDate = @ExpirationDate,
	                                     Notes = @Notes,
	                                     PaidFees = @PaidFees,
	                                     IsActive = @IsActive,
	                                     IssueReason = @IssueReason,
	                                     CreatedByUserID = @CreatedByUserID
                                     WHERE LicenseID = @LicenseID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@DriverID", DriverID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                        Command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        Command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                        if (!string.IsNullOrEmpty(Notes))
                            Command.Parameters.AddWithValue("@Notes", Notes);
                        else
                            Command.Parameters.AddWithValue("@Notes", DBNull.Value);

                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);
                        Command.Parameters.AddWithValue("@IssueReason", IssueReason);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

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

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            int LicenseID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT LicenseID
                                     FROM Licenses
                                     INNER JOIN Drivers
                                     ON Licenses.DriverID = Drivers.DriverID
                                     WHERE Drivers.PersonID = @PersonID
                                     AND Licenses.LicenseClassID = @LicenseClassID
                                     AND Licenses.IsActive = 1";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        Connection.Open();

                        object result = Command.ExecuteScalar();

                        if(result != null && int.TryParse(result.ToString(), out int licenseID))
                        {
                            LicenseID = licenseID;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LicenseID;
        }

        public static bool IsLicenseIssued(int PersonID, int LicenseClassID)
        {
            bool IsIssued = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM Licenses
                                     INNER JOIN Drivers
                                     ON Licenses.DriverID = Drivers.DriverID
                                     WHERE Licenses.LicenseClassID = @LicenseClassID
                                     AND Drivers.PersonID = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null)
                            IsIssued = true;
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsIssued;
        }

        public static DataTable GetAllLocalLicensesForDriver(int DriverID)
        {
            DataTable LocalLicenses = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT LicenseID AS [License ID], ApplicationID AS [Application ID], LicenseClasses.ClassName AS [Class Name], IssueDate AS [Issue Date], ExpirationDate AS [Expiration Date], IsActive AS [Is Active?]
                                     FROM Licenses
                                     INNER JOIN LicenseClasses
                                     ON Licenses.LicenseClassID = LicenseClasses.LicenseClassID
                                     WHERE DriverID = @DriverID
                                     ORDER BY IsActive DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DriverID", DriverID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                LocalLicenses.Load(Reader);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LocalLicenses;
        }

        public static bool DeactivateCurrentLicense(int LicenseID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Licenses
                                     SET IsActive = 0
                                     WHERE LicenseID = @LicenseID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);

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
    }
}
