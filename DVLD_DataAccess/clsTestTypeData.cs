using System;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsTestTypeData
    {

        public static bool GetTestTypeInfoByID(int TestTypeID, ref string TestTypeTitle, ref string TestTypeDescription, ref float TestTypeFees)
        {
            bool IsFound = false;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT *
                                     FROM TestTypes
                                     WHERE TestTypeID = @TestTypeID";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        Connection.Open();

                        using(SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                TestTypeTitle = (string)Reader["TestTypeTitle"];
                                TestTypeDescription = (string)Reader["TestTypeDescription"];
                                TestTypeFees = Convert.ToSingle(Reader["TestTypeFees"]);
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

        public static DataTable GetAllTestTypes()
        {
            DataTable TestTypes = new DataTable();

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TestTypeID AS ID, TestTypeTitle AS Title, TestTypeDescription AS Description, TestTypeFees AS Fees
                                     FROM TestTypes";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                                TestTypes.Load(Reader);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }

            return TestTypes;
        }

        public static int AddNewTestType(string TestTypeTitle, string TestTypeDescription, float TestTypeFees)
        {
            int TestTypeID = -1;

            try
            {
                using(SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"INSERT INTO TestTypes
                                     (TestTypeTitle, TestTypeDescription, TestTypeFees)
                                     VALUES
                                     (@TestTypeTitle, @TestTypeDescription, @TestTypeFees);
                                     SELECT SCOPE_IDENTITY();";

                    using(SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
                        Command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);
                        Command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);

                        Connection.Open();

                        TestTypeID = Convert.ToInt32(Command.ExecuteScalar());
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine("Database Error: " + ex.Message);
            }

            return TestTypeID;
        }

        public static bool UpdateTestType(int TestTypeID, string TestTypeTitle, string TestTypeDescription, float TestTypeFees)
        {
            int RowsAffected = 0;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"UPDATE TestTypes
                                     SET TestTypeTitle = @TestTypeTitle,
                                         TestTypeDescription = @TestTypeDescription,
                                         TestTypeFees = @TestTypeFees
                                     WHERE TestTypeID = @TestTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                        Command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
                        Command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);
                        Command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);

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
