using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {
        public static DataTable GetAllPeople()
        {
            DataTable People = new DataTable();
             
            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth,
	                                     CASE Gender
		                                     WHEN 0 THEN 'Male'
		                                     ELSE 'Female'
	                                     END AS Gender,
                                     Address, Phone, Email, Countries.NationalityName AS Nationality, ImagePath
                                     FROM People
                                     JOIN Countries
                                     ON People.NationalityCountryID = Countries.CountryID
                                     ORDER BY PersonID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                People.Load(Reader);
                        }
                    }

                }
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return People;
        }

        public static int AddNewPerson(string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, byte Gender, string Address, string Phone, string Email, int NationalityCountryID, string ImagePath)
        {
            int PersonID = -1;
    
            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO People
                                     (NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath)
                                     VALUES
                                     (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @DateOfBirth, @Gender, @Address, @Phone, @Email, @NationalityCountryID, @ImagePath);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        Command.Parameters.AddWithValue("@FirstName", FirstName);
                        Command.Parameters.AddWithValue("@SecondName", SecondName);

                        if (!string.IsNullOrEmpty(ThirdName))
                            Command.Parameters.AddWithValue("@ThirdName", ThirdName);
                        else
                            Command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

                        Command.Parameters.AddWithValue("@LastName", LastName);
                        Command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        Command.Parameters.AddWithValue("@Gender", Gender);
                        Command.Parameters.AddWithValue("@Address", Address);
                        Command.Parameters.AddWithValue("@Phone", Phone);

                        if (!string.IsNullOrEmpty(Email))
                            Command.Parameters.AddWithValue("@Email", Email);
                        else
                            Command.Parameters.AddWithValue("@Email", DBNull.Value);

                        Command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

                        if (!string.IsNullOrEmpty(ImagePath))
                            Command.Parameters.AddWithValue("@ImagePath", ImagePath);
                        else
                            Command.Parameters.AddWithValue("@ImagePath", DBNull.Value);

                        Connection.Open();

                        PersonID = Convert.ToInt32(Command.ExecuteScalar());
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return PersonID;
        }

        public static bool GetPersonInfoByID(int PersonID, ref string NationalNo, ref string FirstName, ref string SecondName, ref string ThirdName,
            ref string LastName, ref DateTime DateOfBirth, ref byte Gender, ref string Address, ref string Phone,
            ref string Email, ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM People
                                     WHERE PersonID = @PersonID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                isFound = true;
                                NationalNo = (string)Reader["NationalNo"];
                                FirstName = (string)Reader["FirstName"];
                                SecondName = (string)Reader["SecondName"];

                                if (Reader["ThirdName"] != DBNull.Value)
                                {
                                    ThirdName = (string)Reader["ThirdName"];
                                }
                                else
                                    ThirdName = "";

                                LastName = (string)Reader["LastName"];
                                DateOfBirth = (DateTime)Reader["DateOfBirth"];
                                Gender = (byte)Reader["Gender"];
                                Address = (string)Reader["Address"];
                                Phone = (string)Reader["Phone"];

                                if (Reader["Email"] != DBNull.Value)
                                {
                                    Email = (string)Reader["Email"];
                                }
                                else
                                    Email = "";

                                NationalityCountryID = (int)Reader["NationalityCountryID"];

                                if (Reader["ImagePath"] != DBNull.Value)
                                {
                                    ImagePath = (string)Reader["ImagePath"];
                                }
                                else
                                    ImagePath = "";
                            }
                        }
                    }
                }

            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return isFound;
        }

        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName, ref string ThirdName,
            ref string LastName, ref DateTime DateOfBirth, ref byte Gender, ref string Address, ref string Phone,
            ref string Email, ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM People
                                     WHERE NationalNo = @NationalNo";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@NationalNo", NationalNo);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                isFound = true;
                                PersonID = (int)Reader["PersonID"];
                                FirstName = (string)Reader["FirstName"];
                                SecondName = (string)Reader["SecondName"];

                                if (Reader["ThirdName"] != DBNull.Value)
                                {
                                    ThirdName = (string)Reader["ThirdName"];
                                }
                                else
                                    ThirdName = "";

                                LastName = (string)Reader["LastName"];
                                DateOfBirth = (DateTime)Reader["DateOfBirth"];
                                Gender = (byte)Reader["Gender"];
                                Address = (string)Reader["Address"];
                                Phone = (string)Reader["Phone"];

                                if (Reader["Email"] != DBNull.Value)
                                {
                                    Email = (string)Reader["Email"];
                                }
                                else
                                    Email = "";

                                NationalityCountryID = (int)Reader["NationalityCountryID"];

                                if (Reader["ImagePath"] != DBNull.Value)
                                {
                                    ImagePath = (string)Reader["ImagePath"];
                                }

                                else
                                    ImagePath = "";
                            }
                        }
                    }
                }

            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return isFound;
        }

        public static bool UpdatePerson(int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName,
            string LastName, DateTime DateOfBirth, byte Gender, string Address, string Phone,
            string Email, int NationalityCountryID, string ImagePath)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE People
                                     SET NationalNo = @NationalNo,
	                                     FirstName = @FirstName,
	                                     SecondName = @SecondName,
	                                     ThirdName = @ThirdName,
	                                     LastName = @LastName,
	                                     DateOfBirth = @DateOfBirth,
	                                     Gender = @Gender,
	                                     Address = @Address,
	                                     Phone = @Phone,
	                                     Email = @Email,
	                                     NationalityCountryID = @NationalityCountryID,
	                                     ImagePath = @ImagePath
                                     WHERE PersonID = @PersonID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        Command.Parameters.AddWithValue("@FirstName", FirstName);
                        Command.Parameters.AddWithValue("@SecondName", SecondName);

                        if (!string.IsNullOrEmpty(ThirdName))
                            Command.Parameters.AddWithValue("@ThirdName", ThirdName);
                        else
                            Command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

                        Command.Parameters.AddWithValue("@LastName", LastName);
                        Command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        Command.Parameters.AddWithValue("@Gender", Gender);
                        Command.Parameters.AddWithValue("@Address", Address);
                        Command.Parameters.AddWithValue("@Phone", Phone);

                        if (!string.IsNullOrEmpty(Email))
                            Command.Parameters.AddWithValue("@Email", Email);
                        else
                            Command.Parameters.AddWithValue("@Email", DBNull.Value);

                        Command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

                        if (!string.IsNullOrEmpty(ImagePath))
                            Command.Parameters.AddWithValue("@ImagePath", ImagePath);
                        else
                            Command.Parameters.AddWithValue("@ImagePath", DBNull.Value);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();

                    }
                }
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }

        public static bool DeletePerson(int PersonID)
        {
            int RowsAffected = 0;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"DELETE FROM People
                                     WHERE PersonID = @PersonID";

                    using(SqlCommand Command =  new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        RowsAffected = Command.ExecuteNonQuery();
                    }
                }
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return (RowsAffected > 0);
        }

        public static bool IsPersonExists(int PersonID)
        {
            bool IsExists = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Found = 'TRUE'
                                     FROM People
                                     WHERE PersonID = @PersonID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if(Reader.Read())
                                IsExists = true;
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return IsExists;
        }

        public static bool IsPersonExists(string NationalNo)
        {
            bool IsExists = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT Found = 'TRUE'
                                     FROM People
                                     WHERE NationalNo = @NationalNo";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@NationalNo", NationalNo);

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

    }
}
