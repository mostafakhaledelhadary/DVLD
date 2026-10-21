using System;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsTestData
    {
        public static bool GetTestInfoByTestID(int TestID, ref int TestAppointmentID, ref bool TestResult,
            ref string Notes, ref int CreatedByUserID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Tests
                                     WHERE TestID = @TestID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestID", TestID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                TestAppointmentID = Convert.ToInt32(Reader["TestAppointmentID"]);
                                TestResult = Convert.ToBoolean(Reader["TestResult"]);
                                Notes = Convert.ToString(Reader["Notes"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return IsFound;
        }

        public static int AddNewTest(int TestAppointmentID, bool TestResult, string Notes, int CreatedByUserID)
        {
            int TestID = -1;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO Tests
                                     (TestAppointmentID, TestResult, Notes, CreatedByUserID)
                                     VALUES
                                     (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command =  new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                        Command.Parameters.AddWithValue("@TestResult", TestResult);

                        if (string.IsNullOrEmpty(Notes))
                            Command.Parameters.AddWithValue("@Notes", DBNull.Value);
                        else
                            Command.Parameters.AddWithValue("@Notes", Notes);

                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if(int.TryParse(Result.ToString(), out int InsertedID))
                        {
                            TestID = InsertedID;
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return TestID;
        }

        public static bool UpdateTest(int TestID, int TestAppointmentID, bool TestResult, string Notes, int CreatedByUserID)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Tests
                                     SET TestAppointmentID = @TestAppointmentID,
	                                     TestResult = @TestResult,
	                                     Notes = @Notes,
	                                     CreatedByUserID = @CreatedByUserID
                                     WHERE TestID = @TestID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestID", TestID);
                        Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                        Command.Parameters.AddWithValue("@TestResult", TestResult);

                        if (string.IsNullOrEmpty(Notes))
                            Command.Parameters.AddWithValue("@Notes", DBNull.Value);
                        else
                            Command.Parameters.AddWithValue("@Notes", Notes);

                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
                return false;
            }

            return (RowsAffected > 0);
        }

        public static byte GetNumberOfPassedTests(int LocalDrivingLicenseApplicationID)
        {
            byte NumberOfPassedTests = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT COUNT(1) AS NumberOfPassedTests
                                     FROM Tests
                                     INNER JOIN TestAppointments
                                     ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                     WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND Tests.TestResult = 1";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if(Result != null && byte.TryParse(Result.ToString(), out byte NumOfPassedTests))
                        {
                            NumberOfPassedTests = NumOfPassedTests;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return NumberOfPassedTests;
        }

        public static bool DoesPersonPassInTest(int LocalDrivingLicenseApplicationID, byte TestTypeID)
        {
            bool IsPassed = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM Tests
                                     JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                     WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND Tests.TestResult = 1
                                     AND TestAppointments.TestTypeID = @TestTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        IsPassed = (Command.ExecuteScalar() != null);
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsPassed;
        }

        public static byte GetTestTrials(int LocalDrivingLicenseApplicationID, byte TestTypeID)
        {
            byte TestTrials = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT COUNT(*) AS Trials
                                     FROM Tests
                                     JOIN TestAppointments ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                                     WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND TestAppointments.TestTypeID = @TestTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null && byte.TryParse(Result.ToString(), out byte NumOfTestTrials))
                        {
                            TestTrials = NumOfTestTrials;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return TestTrials;
        }

        public static bool GetLastTestByPersonIDAndLicenseClassID(int ApplicantPersonID, int LicenseClassID, byte TestTypeID,
            ref int TestID, ref int TestAppointmentID, ref bool TestResult, ref string Notes, ref int CreatedByUserID)
        {
            bool IsFound = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 TestID, Tests.TestAppointmentID, TestResult, Notes, Tests.CreatedByUserID, Applications.ApplicantPersonID
                                     FROM Tests
                                     INNER JOIN TestAppointments
                                     ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                     INNER JOIN LocalDrivingLicenseApplications
                                     ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                                     INNER JOIN Applications
                                     ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
                                     WHERE Applications.ApplicantPersonID = @ApplicantPersonID
                                     AND LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID
                                     AND TestAppointments.TestTypeID = @TestTypeID
                                     ORDER BY Tests.TestAppointmentID DESC";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                TestID = Convert.ToInt32(Reader["TestID"]);
                                TestAppointmentID = Convert.ToInt32(Reader["TestAppointmentID"]);
                                TestResult = Convert.ToBoolean(Reader["TestResult"]);
                                Notes = Convert.ToString(Reader["Notes"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                            }
                        }
                    }
                }

            }

            catch(Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return IsFound;
        }

        public static int GetTestID(int TestAppointmentID)
        {
            int TestID = -1;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TestID
                                     FROM Tests
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if(Result != null && int.TryParse(Result.ToString(), out int InsertedID))
                        {
                            TestID = InsertedID;
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return TestID;
        }
    }
}
