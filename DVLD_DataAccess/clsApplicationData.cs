using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsApplicationData
    {
        public static bool GetApplicationInfoByApplicationID(int ApplicationID, ref int ApplicantPersonID,
            ref DateTime ApplicationDate, ref int ApplicationTypeID, ref byte ApplicationStatus,
            ref DateTime LastStatusDate, ref float PaidFees, ref int CreatedByUserID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Applications
                                     WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                ApplicantPersonID = (int)Reader["ApplicantPersonID"];
                                ApplicationDate = (DateTime)Reader["ApplicationDate"];
                                ApplicationTypeID = (int)Reader["ApplicationTypeID"];
                                ApplicationStatus = (byte)Reader["ApplicationStatus"];
                                LastStatusDate = (DateTime)Reader["LastStatusDate"];
                                PaidFees = Convert.ToSingle(Reader["PaidFees"]);
                                CreatedByUserID = (int)Reader["CreatedByUserID"];
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

        public static bool IsApplicationExists(int ApplicationID)
        {
            bool IsExists = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Found = 'TRUE'
                                     FROM Applications
                                     WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                                IsExists = true;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsExists;
        }

        public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate,
            int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate,
            float PaidFees, int CreatedByUserID)
        {
            int ApplicationID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO Applications
                                     (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                                     VALUES
                                     (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        Command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        Command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                        Command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        Connection.Open();

                        ApplicationID = Convert.ToInt32(Command.ExecuteScalar());
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return ApplicationID;
        }

        public static bool UpdateApplication(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate,
            int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate,
            float PaidFees, int CreatedByUserID)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Applications
                                     SET ApplicantPersonID = @ApplicantPersonID,
                                         ApplicationDate = @ApplicationDate,
                                         ApplicationTypeID = @ApplicationTypeID,
                                         ApplicationStatus = @ApplicationStatus,
                                         LastStatusDate = @LastStatusDate,
                                         PaidFees = @PaidFees,
                                         CreatedByUserID = @CreatedByUserID
                                     WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        Command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        Command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                        Command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID, int ApplicationTypeID, int LicenseClassID)
        {
            int ActiveApplicationID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Applications.ApplicationID
                                     FROM Applications
                                     JOIN People ON Applications.ApplicantPersonID = People.PersonID
                                     JOIN LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                                     JOIN LicenseClasses ON LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID
                                     WHERE People.PersonID = @PersonID
                                     AND Applications.ApplicationTypeID = @ApplicationTypeID
                                     AND LicenseClasses.LicenseClassID = @LicenseClassID
                                     AND Applications.ApplicationStatus = 1";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if(Result != null && int.TryParse(Result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return ActiveApplicationID;
        }

        public static bool UpdateApplicationStatus(int ApplicationID, byte NewStatus)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Applications
                                     SET ApplicationStatus = @NewStatus,
                                         LastStatusDate = @LastStatusDate
                                     WHERE ApplicationID = @ApplicationID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@NewStatus", NewStatus);
                        Command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);

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

        public static bool DeleteApplication(int ApplicationID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"DELETE
                                     FROM Applications
                                     WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

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
