using System;
using System.Data.SqlClient;
using System.Data;
using DVLD.Global_Classes;

namespace DVLD_DataAccess
{
    public class clsUserData
    {
        public static bool GetUserInfoByUserID(int UserID, ref int PersonID, ref string Username, ref string Password, ref bool IsActive)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Users
                                     WHERE UserID = @UserID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@UserID", UserID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                PersonID = (int)Reader["PersonID"];
                                Username = (string)Reader["Username"];
                                Password = (string)Reader["Password"];
                                IsActive = (bool)Reader["IsActive"];
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                clsEventLogger.LogException(ex);
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsFound;
        }

        public static bool GetUserInfoByPersonID(int PersonID, ref int UserID, ref string Username, ref string Password, ref bool IsActive)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Users
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
                                UserID = (int)Reader["UserID"];
                                Username = (string)Reader["Username"];
                                Password = (string)Reader["Password"];
                                IsActive = (bool)Reader["IsActive"];
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

        public static bool GetUserInfoByUsernameAndPassword(string Username, string Password, ref int UserID, ref int PersonID, ref bool IsActive)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM Users
                                     WHERE Username = @Username
                                     AND Password = @Password";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@Username", Username);
                        Command.Parameters.AddWithValue("@Password", Password);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                UserID = (int)Reader["UserID"];
                                PersonID = (int)Reader["PersonID"];
                                IsActive = (bool)Reader["IsActive"];
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

        public static int AddNewUser(int PersonID, string Username, string Password, bool IsActive)
        {
            int UserID = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO Users
                                     (PersonID, Username, Password, IsActive)
                                     VALUES
                                     (@PersonID, @Username, @Password, @IsActive);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@Username", Username);
                        Command.Parameters.AddWithValue("@Password", Password);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);

                        Connection.Open();

                        object Result = Command.ExecuteScalar();

                        if (Result != null && int.TryParse(Result.ToString(), out int InsertedID))
                        {
                            UserID = InsertedID;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return UserID;
        }

        public static bool UpdateUser(int UserID, int PersonID, string Username, string Password, bool IsActive)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE Users
                                     SET PersonID = @PersonID,
                                         Username = @Username,
	                                     Password = @Password,
	                                     IsActive = @IsActive
                                     WHERE UserID = @UserID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@UserID", UserID);
                        Command.Parameters.AddWithValue("@Username", Username);
                        Command.Parameters.AddWithValue("@Password", Password);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);

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

        public static bool DeleteUser(int UserID)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"DELETE
                                     FROM Users
                                     WHERE UserID = @UserID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@UserID", UserID);

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

        public static bool IsUserExists(int UserID)
        {
            bool IsExists = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Found = 'TRUE'
                                     FROM Users
                                     WHERE UserID = @UserID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@UserID", UserID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
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

        public static bool IsUserExists(string Username)
        {
            bool IsExists = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Found = 'TRUE'
                                     FROM Users
                                     WHERE Username = @Username";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@Username", Username);

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

        public static bool IsPersonIsAUser(int PersonID)
        {
            bool IsExists = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Found = 'TRUE'
                                     FROM Users
                                     WHERE PersonID = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

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

        public static DataTable GetAllUsers()
        {
            DataTable Users = new DataTable();

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT UserID AS [User ID], Users.PersonID AS [Person ID],

                                     CASE
                                          WHEN ThirdName IS NULL THEN FirstName + ' ' + SecondName + ' ' + LastName
                                          ELSE FirstName + ' ' + SecondName + ' ' + ThirdName + ' ' + LastName
                                     END
                                          AS [Full Name],
                                     
                                     Username, IsActive AS [Is Active?]
                                     FROM Users
                                     JOIN People
                                     ON Users.PersonID = People.PersonID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();
                        
                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                Users.Load(Reader);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return Users;
        }

    }
}
