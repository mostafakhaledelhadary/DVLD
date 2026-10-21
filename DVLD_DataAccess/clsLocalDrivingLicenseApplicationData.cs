using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsLocalDrivingLicenseApplicationData
    {
        public static bool GetLocalDrivingLicenseApplicationInfoByID(int LocalDrivingLicenseApplicationID,
            ref int ApplicationID, ref int LicenseClassID)
        {
            bool IsFound = false;

            try 
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM LocalDrivingLicenseApplications
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                ApplicationID = (int)Reader["ApplicationID"];
                                LicenseClassID = (int)Reader["LicenseClassID"];
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

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            DataTable LocalDrivingLicenseApplications = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM LocalDrivingLicenseApplications_View";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                LocalDrivingLicenseApplications.Load(Reader);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LocalDrivingLicenseApplications;
        }

        public static int AddNewLocalDrivingLicenseApplication(int ApplicationID, int LicenseClassID)
        {
            int LocalDrivingLicenseApplicationID = -1;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO LocalDrivingLicenseApplications
                                     (ApplicationID, LicenseClassID)
                                     VALUES
                                     (@ApplicationID, @LicenseClassID);
                                     SELECT SCOPE_IDENTITY();";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if(Result != null && int.TryParse(Result.ToString(), out int LocalDrivingLicenseAppID))
                        {
                            LocalDrivingLicenseApplicationID = LocalDrivingLicenseAppID;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return LocalDrivingLicenseApplicationID;
        }

        public static bool UpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int ApplicationID, int LicenseClassID)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE LocalDrivingLicenseApplications
                                     SET ApplicationID = @ApplicationID,
	                                     LicenseClassID = @LicenseClassID
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

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

        public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"DELETE FROM LocalDrivingLicenseApplications
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                        Connection.Open();

                        RowsAffected = Convert.ToInt32(Command.ExecuteNonQuery());
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }

        public static bool IsLocalDrivingLicenseApplicationExists(int LocalDrivingLicenseApplicationID)
        {
            bool IsExists = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM LocalDrivingLicenseApplications
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null)
                            IsExists = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsExists;
        }

        public static bool DoesPassTestType(int LocalDrivingLicenseApplicationID, byte TestTypeID)
        {
            bool IsPassed = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM Tests
                                     INNER JOIN TestAppointments
                                     ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                     WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND TestAppointments.TestTypeID = @TestTypeID
                                     AND Tests.TestResult = 1";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null)
                            IsPassed = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return IsPassed;
        }

        public static bool DoesAttendTestType(int LocalDrivingLicenseApplicationID, byte TestTypeID)
        {
            bool IsAttend = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM TestAppointments
                                     INNER JOIN Tests
                                     ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                                     WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND TestAppointments.TestTypeID = @TestTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null)
                            IsAttend = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return IsAttend;
        }

        public static bool IsThereAnActiveScheduledTest(int LocalDrivingLicenseApplicationID, byte TestTypeID)
        {
            bool IsActive = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM TestAppointments
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND TestTypeID = @TestTypeID
                                     AND IsLocked = 0";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if(Result != null)
                        {
                            IsActive = true;
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return IsActive;
        }

    }
}
