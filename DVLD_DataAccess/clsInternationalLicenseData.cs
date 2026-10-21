using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;


namespace DVLD_DataAccess
{
    public class clsInternationalLicenseData
    {
        public static bool GetInternationalLicenseInfoByID(int InternationalLicenseID, ref int ApplicationID, ref int DriverID,
           ref int IssuedUsingLocalLicenseID, ref DateTime IssueDate,
           ref DateTime ExpirationDate, ref bool IsActive, ref int CreatedByUserID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM InternationalLicenses
                                     WHERE InternationalLicenseID = @InternationalLicenseID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                ApplicationID = Convert.ToInt32(Reader["ApplicationID"]);
                                DriverID = Convert.ToInt32(Reader["DriverID"]);
                                IssuedUsingLocalLicenseID = Convert.ToInt32(Reader["IssuedUsingLocalLicenseID"]);
                                IssueDate = Convert.ToDateTime(Reader["IssueDate"]);
                                ExpirationDate = Convert.ToDateTime(Reader["ExpirationDate"]);
                                IsActive = Convert.ToBoolean(Reader["IsActive"]);
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

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            int InternationalLicenseID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 InternationalLicenseID
                                     FROM InternationalLicenses
                                     WHERE DriverID = @DriverID
                                     AND IsActive = 1
                                     ORDER BY InternationalLicenseID DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DriverID", DriverID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null && int.TryParse(Result.ToString(), out int insertedID))
                            InternationalLicenseID = insertedID;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return InternationalLicenseID;
        }

        public static int AddNewInternationalLicense(int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID, DateTime IssueDate,
         DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            int InternationalLicenseID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO InternationalLicenses
                                     (ApplicationID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive, CreatedByUserID)
                                     VALUES
                                     (@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID, @IssueDate, @ExpirationDate, @IsActive, @CreatedByUserID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@DriverID", DriverID);
                        Command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                        Command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        Command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        Connection.Open();

                        InternationalLicenseID = Convert.ToInt32(Command.ExecuteScalar());
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return InternationalLicenseID;
        }

        public static bool UpdateInternationalLicense(int InternationalLicenseID, int ApplicationID, int DriverID,
           int IssuedUsingLocalLicenseID, DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE InternationalLicenses
                                     SET ApplicationID = @ApplicationID,
	                                     DriverID = @DriverID,
	                                     IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID,
	                                     IssueDate = @IssueDate,
	                                     ExpirationDate = @ExpirationDate,
	                                     IsActive = @IsActive,
	                                     CreatedByUserID = @CreatedByUserID
                                     WHERE InternationalLicenseID = @InternationalLicenseID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@DriverID", DriverID);
                        Command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                        Command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        Command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);
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

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            DataTable InternationalLicenses = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT InternationalLicenseID AS [International License ID], ApplicationID AS [Application ID], IssuedUsingLocalLicenseID AS [Local License ID], IssueDate AS [Issue Date], ExpirationDate AS [Expiration Date], IsActive AS [Is Active?]
                                     FROM InternationalLicenses
                                     WHERE DriverID = @DriverID
                                     ORDER BY IsActive DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DriverID", DriverID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                InternationalLicenses.Load(Reader);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return InternationalLicenses;
        }

        public static DataTable GetAllInternationalLicenses()
        {
            DataTable InternationalLicenses = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT InternationalLicenseID AS [International License ID], ApplicationID AS [Application ID], DriverID AS [Driver ID], IssuedUsingLocalLicenseID AS [Local License ID], IssueDate AS [Issue Date], ExpirationDate AS [Expiration Date], IsActive AS [Is Active?]
                                     FROM InternationalLicenses";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                InternationalLicenses.Load(Reader);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return InternationalLicenses;
        }
    }
}