using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsTestAppointmentData
    {
        public static bool GetTestAppointmentInfoByID(int TestAppointmentID, ref byte TestTypeID,
            ref int LocalDrivingLicenseApplicationID, ref DateTime AppointmentDate,
            ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM TestAppointments
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;

                                // استخدام Convert أمان 100% لمنع InvalidCastException
                                TestTypeID = Convert.ToByte(Reader["TestTypeID"]);
                                LocalDrivingLicenseApplicationID = Convert.ToInt32(Reader["LocalDrivingLicenseApplicationID"]);
                                AppointmentDate = Convert.ToDateTime(Reader["AppointmentDate"]);
                                PaidFees = Convert.ToSingle(Reader["PaidFees"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                                IsLocked = Convert.ToBoolean(Reader["IsLocked"]);

                                if (Reader["RetakeTestApplicationID"] == DBNull.Value)
                                    RetakeTestApplicationID = -1;
                                else
                                    RetakeTestApplicationID = Convert.ToInt32(Reader["RetakeTestApplicationID"]);
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

        public static bool GetLastTestAppointment(int LocalDrivingLicenseApplicationID, byte TestTypeID, ref int TestAppointmentID,
            ref DateTime AppointmentDate, ref float PaidFees, ref int CreatedByUserID,
            ref bool IsLocked, ref int RetakeTestApplicationID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 *
                                     FROM TestAppointments
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND TestTypeID = @TestTypeID
                                     ORDER BY TestAppointmentID DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                TestAppointmentID = Convert.ToInt32(Reader["TestAppointmentID"]);
                                AppointmentDate = Convert.ToDateTime(Reader["AppointmentDate"]);
                                PaidFees = Convert.ToSingle(Reader["PaidFees"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                                IsLocked = Convert.ToBoolean(Reader["IsLocked"]);

                                if (Reader["RetakeTestApplicationID"] == DBNull.Value)
                                    RetakeTestApplicationID = -1;
                                else
                                    RetakeTestApplicationID = Convert.ToInt32(Reader["RetakeTestApplicationID"]);
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

        public static int AddNewTestAppointment(byte TestTypeID, int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate, float PaidFees,
            int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            int TestAppointmentID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO TestAppointments
                                     (TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID)
                                     VALUES
                                     (@TestTypeID, @LocalDrivingLicenseApplicationID, @AppointmentDate, @PaidFees, @CreatedByUserID, @IsLocked, @RetakeTestApplicationID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@IsLocked", IsLocked);

                        if (RetakeTestApplicationID == -1)
                            Command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
                        else
                            Command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null && int.TryParse(Result.ToString(), out int InsertedID))
                        {
                            TestAppointmentID = InsertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return TestAppointmentID;
        }

        public static bool UpdateTestAppointment(int TestAppointmentID, byte TestTypeID, int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate, float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE TestAppointments
                                     SET TestTypeID = @TestTypeID,
                                        LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID,
                                        AppointmentDate = @AppointmentDate,
                                        PaidFees = @PaidFees,
                                        CreatedByUserID = @CreatedByUserID,
                                        IsLocked = @IsLocked,
                                        RetakeTestApplicationID = @RetakeTestApplicationID
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@IsLocked", IsLocked);

                        if (RetakeTestApplicationID == -1)
                            Command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
                        else
                            Command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID);

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

        public static DataTable GetAllTestAppointmentsForTestType(int LocalDrivingLicenseApplicationID, byte TestTypeID)
        {
            DataTable TestAppointments = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TestAppointmentID AS [Appointment ID],
                                        AppointmentDate AS [Appointment Date],
                                        PaidFees AS [Paid Fees],
                                        IsLocked AS [Is Locked]
                                     FROM TestAppointments
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     AND TestTypeID = @TestTypeID
                                     ORDER BY TestAppointmentID DESC";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                TestAppointments.Load(Reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return TestAppointments;
        }

        public static bool LockAppointment(int TestAppointmentID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE TestAppointments
                                     SET IsLocked = 1
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

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