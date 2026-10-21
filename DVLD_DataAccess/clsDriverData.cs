using System;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsDriverData
    {
        public static bool GetDriverInfoByDriverID(int DriverID, ref int PersonID, ref int CreatedByUserID, ref DateTime CreatedDate)
        {
            bool IsFound = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Drivers
                                     WHERE DriverID = @DriverID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DriverID", DriverID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                PersonID = Convert.ToInt32(Reader["PersonID"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                                CreatedDate = Convert.ToDateTime(Reader["CreatedDate"]);
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

        public static bool GetDriverInfoByPersonID(int PersonID, ref int DriverID, ref int CreatedByUserID, ref DateTime CreatedDate)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Drivers
                                     WHERE PersonID = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                DriverID = Convert.ToInt32(Reader["DriverID"]);
                                CreatedByUserID = Convert.ToInt32(Reader["CreatedByUserID"]);
                                CreatedDate = Convert.ToDateTime(Reader["CreatedDate"]);
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

        public static int AddNewDriver(int PersonID, int CreatedByUserID, DateTime CreatedDate)
        {
            int DriverID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO Drivers
                                     (PersonID, CreatedByUserID, CreatedDate)
                                     VALUES
                                     (@PersonID, @CreatedByUserID, @CreatedDate);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@CreatedDate", CreatedDate);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null && int.TryParse(Result.ToString(), out int InsertedID))
                        {
                            DriverID = InsertedID;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return DriverID;
        }

        public static bool UpdateDriver(int DriverID, int PersonID, int CreatedByUserID, DateTime CreatedDate)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Drivers
                                     SET PersonID = @PersonID,
	                                     CreatedByUserID = @CreatedByUserID,
	                                     CreatedDate = @CreatedDate
                                     WHERE DriverID = @DriverID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@DriverID", DriverID);
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@CreatedDate", CreatedDate);

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

        public static DataTable GetAllDrivers()
        {
            DataTable Drivers = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT DriverID AS [Driver ID], Drivers.PersonID AS [Person ID], People.NationalNo AS [National No.], FirstName + ' ' + SecondName + ' ' + ISNULL(ThirdName + ' ', '') + LastName AS [Full Name], CreatedDate AS [Date],
                                     (SELECT COUNT(1) FROM Licenses WHERE Licenses.DriverID = Drivers.DriverID AND Licenses.IsActive = 1) AS [Active Licenses]
                                     FROM Drivers
                                     INNER JOIN People
                                     ON Drivers.PersonID = People.PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                Drivers.Load(Reader);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return Drivers;
        }

        public static bool IsPersonADriver(int PersonID)
        {
            bool IsDriver = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1
                                     FROM Drivers
                                     WHERE PersonID = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null)
                            IsDriver = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsDriver;
        }
    }
}
