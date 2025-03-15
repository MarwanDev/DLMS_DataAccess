using System;
using System.Data.SqlClient;
using System.Data;

namespace DLMS_DataAccess
{
    public class ApplicationTypeData : Data
    {
        public static DataTable GetAllApplicationTypes()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT  [ApplicationTypeID]\r\n   " +
                ",[ApplicationTypeTitle]\r\n    " +
                ",[ApplicationFees]\r\n " +
                "FROM [dvld].[dbo].[ApplicationTypes]" + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dt.Columns["ApplicationTypeID"].ColumnName = "ID";
                    dt.Columns["ApplicationTypeTitle"].ColumnName = "Title";
                    dt.Columns["ApplicationFees"].ColumnName = "Fees";
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

        public static int GetAllApplicationTypesCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select count (*) from\n " +
                "ApplicationTypes";
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

        public static bool GetApplicationTypeDataById(int id, ref string title, ref decimal fees)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [ApplicationTypeID]\r\n   " +
                ",[ApplicationTypeTitle]\r\n" +
                ",[ApplicationFees]\r\n" +
                "FROM [dvld].[dbo].[ApplicationTypes]\n" +
                "Where ApplicationTypeID = @ApplicationTypeID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationTypeID", id);

            try
            {
                // The record was found
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    title = (string)reader["ApplicationTypeTitle"];
                    fees = (decimal)reader["ApplicationFees"];
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

        public static bool UpdateApplicationType(int id, string title, decimal fees)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  ApplicationTypes  
                            set ApplicationTypeTitle = @ApplicationTypeTitle, 
                                ApplicationFees = @ApplicationFees
                                where ApplicationTypeID = @ApplicationTypeID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationTypeID", id);
            command.Parameters.AddWithValue("@ApplicationFees", fees);
            command.Parameters.AddWithValue("@ApplicationTypeTitle", title);

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
