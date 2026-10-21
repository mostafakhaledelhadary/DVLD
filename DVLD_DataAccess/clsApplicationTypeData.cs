using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;


namespace DVLD_DataAccess
{
    public class clsApplicationTypeData
    {
        public static bool GetApplicationTypeInfoByID(int ApplicationTypeID, ref string ApplicationTypeTitle, ref float ApplicationFees)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM ApplicationTypes
                                     WHERE ApplicationTypeID = @ApplicationTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                       
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                ApplicationTypeTitle = (string)Reader["ApplicationTypeTitle"];
                                ApplicationFees = Convert.ToSingle(Reader["ApplicationFees"]);
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

        public static DataTable GetAllApplicationTypes()
        {
            DataTable ApplicationTypes = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT ApplicationTypeID AS ID, ApplicationTypeTitle AS Title, ApplicationFees AS Fees
                                     FROM ApplicationTypes";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                ApplicationTypes.Load(Reader);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return ApplicationTypes;
        }

        public static int AddNewApplicationType(string ApplicationTypeTitle, float ApplicationFees)
        {
            int ApplicationTypeID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO ApplicationTypes
                                     (ApplicationTypeTitle, ApplicationFees)
                                     VALUES
                                     (@ApplicationTypeTitle, @ApplicationFees);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                        Command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

                        Connection.Open();

                        ApplicationTypeID = Convert.ToInt32(Command.ExecuteScalar());
                    }                          
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return ApplicationTypeID;
        }

        public static bool UpdateApplicationType(int ApplicationTypeID, string ApplicationTypeTitle, float ApplicationFees)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE ApplicationTypes
                                     SET ApplicationTypeTitle = @ApplicationTypeTitle,
                                         ApplicationFees = @ApplicationFees
                                     WHERE ApplicationTypeID = @ApplicationTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        Command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                        Command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

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
