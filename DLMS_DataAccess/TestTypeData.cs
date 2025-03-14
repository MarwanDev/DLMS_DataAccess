using System;
using System.Data.SqlClient;
using System.Data;

namespace DLMS_DataAccess
{
    public class TestTypeData
    {
        public static DataTable GetAllTestTypes()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT  [TestTypeID]\r\n   " +
                ",[TestTypeTitle]\r\n    " +
                ",[TestTypeDescription]\r\n    " +
                ",[TestTypeFees]\r\n " +
                "FROM [dvld].[dbo].[TestTypes]" + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dt.Columns["TestTypeID"].ColumnName = "ID";
                    dt.Columns["TestTypeTitle"].ColumnName = "Title";
                    dt.Columns["TestTypeDescription"].ColumnName = "Description";
                    dt.Columns["TestTypeFees"].ColumnName = "Fees";
                }
                reader.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static string SortingText { set; get; }
        private static string sortingType = "DESC";
        private static string currentSortingText;
        public static bool IsSortingUsed { get; set; }
        private static string GetSortingCommand()
        {
            sortingType = currentSortingText == SortingText ? sortingType == "ASC" ? "DESC" : "ASC" : "ASC";
            currentSortingText = SortingText;
            return $"\nORDER BY {SortingText} {sortingType}";
        }

        public static void ApplySorting()
        {
            SortingCondition = IsSortingUsed ? GetSortingCommand() : "";
        }

        private static string SortingCondition;

        public static int GetAllTestTypesCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select count (*) from\n " +
                "[TestTypes]";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    count = (int)(dt.Rows[0][0] ?? null);
                }
                reader.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }

            return count;
        }

        public static bool GetTestTypeDataById(int id, ref string title, ref decimal fees, ref string description)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [TestTypeID]\r\n   " +
                ",[TestTypeTitle]\r\n" +
                ",[TestTypeDescription]\r\n" +
                ",[TestTypeFees]\r\n" +
                "FROM [dvld].[dbo].[TestTypes]\n" +
                "Where TestTypeID = @TestTypeID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TestTypeID", id);

            try
            {
                // The record was found
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    title = (string)reader["TestTypeTitle"];
                    description = (string)reader["TestTypeDescription"];
                    fees = (decimal)reader["TestTypeFees"];
                }
                else
                {
                    // The record was not found
                    isFound = false;
                }
            }
            catch (Exception)
            {
                isFound = false;
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }

        public static bool UpdateTestType(int id, string title, decimal fees, string description)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  [TestTypes]  
                            set TestTypeTitle = @TestTypeTitle, 
                                TestTypeFees = @TestTypeFees
                                TestTypeDescription = @TestTypeDescription
                                where TestTypeID = @TestTypeID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TestTypeID", id);
            command.Parameters.AddWithValue("@TestTypeFees", fees);
            command.Parameters.AddWithValue("@TestTypeTitle", title);
            command.Parameters.AddWithValue("@TestTypeDescription", description);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
            return (rowsAffected > 0);
        }
    }
}
