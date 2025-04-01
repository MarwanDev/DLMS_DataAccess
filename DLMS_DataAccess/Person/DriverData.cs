using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Policy;

namespace DLMS_DataAccess.Person
{
    public class DriverData : Data
    {
        public static int AddNewDriver(int personId, int createdByUserId, DateTime createdDate)
        {
            int licenceId = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Drivers (PersonId, CreatedByUserId, CreatedDate)
                            VALUES (@PersonId, @CreatedByUserId, @CreatedDate);
                            SELECT SCOPE_IDENTITY();"
            ;

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonId", personId);
            command.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);
            command.Parameters.AddWithValue("@CreatedDate", createdDate);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    licenceId = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return licenceId;
        }

        public static DataTable GetAllDrivers()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n    " +
                "D.[DriverID] AS 'Driver ID',\r\n    " +
                "D.[PersonID] AS 'Person ID',\r\n    " +
                "P.[Nationalno] AS 'National No.',\r\n    " +
                "concat(P.FirstName, ' ', P.SecondName, ' ', P.ThirdName, ' ', P.LastName) as 'Full Name',\r\n    " +
                "D.[CreatedDate] AS 'Date',\r\n    " +
                "COUNT(L.[LicenseID]) AS 'Active Licenses'\r\n" +
                "FROM [dvld].[dbo].[Drivers] D\r\n" +
                "JOIN [dvld].[dbo].[People] P ON P.PersonID = D.PersonID\r\n" +
                "LEFT JOIN [dvld].[dbo].[Licenses] L \r\n    ON D.DriverID = L.DriverID \r\n    " +
                "AND L.IsActive = 1\r\nGROUP BY D.DriverID, D.PersonID, D.CreatedDate,\r\n" +
                "P.NationalNo, P.FirstName, P.SecondName, P.ThirdName, P.LastName" + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
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

        public static string CurrentFilter;

        private static string GetFilterConditionText(string filterkeyWord)
        {
            string filterCondition = $"\"{CurrentFilter}\" like '%{filterkeyWord}%'";
            return filterCondition.Length > 0 ? "\nwhere " + filterCondition : "";
        }

        public static DataTable GetFilteredDrivers(string filterkeyWord)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n    " +
                "D.[DriverID] AS 'Driver ID',\r\n    " +
                "D.[PersonID] AS 'Person ID',\r\n    " +
                "P.[Nationalno] AS 'National No.',\r\n    " +
                "concat(P.FirstName, ' ', P.SecondName, ' ', P.ThirdName, ' ', P.LastName) as 'Full Name',\r\n    " +
                "D.[CreatedDate] AS 'Date',\r\n    " +
                "COUNT(L.[LicenseID]) AS 'Active Licenses'\r\n" +
                "FROM [dvld].[dbo].[Drivers] D\r\n" +
                "JOIN [dvld].[dbo].[People] P ON P.PersonID = D.PersonID\r\n" +
                "LEFT JOIN [dvld].[dbo].[Licenses] L \r\n    ON D.DriverID = L.DriverID \r\n    " +
                "AND L.IsActive = 1\r\nGROUP BY D.DriverID, D.PersonID, D.CreatedDate,\r\n" +
                "P.NationalNo, P.FirstName, P.SecondName, P.ThirdName, P.LastName" + GetFilterConditionText(filterkeyWord) + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
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

        public static int GetAllDriversCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "select count(*) from " +
                "(\r\n\r\nSELECT \r\n    " +
                "D.[DriverID] AS 'Driver ID',\r\n    " +
                "D.[PersonID] AS 'Person ID',\r\n    " +
                "P.[Nationalno] AS 'National No.',\r\n    " +
                "concat(P.FirstName, ' ', P.SecondName, ' ', P.ThirdName, ' ', P.LastName) as 'Full Name',\r\n    " +
                "D.[CreatedDate] AS 'Date',\r\n    " +
                "COUNT(L.[LicenseID]) AS 'Active Licenses'\r\n" +
                "FROM [dvld].[dbo].[Drivers] D\r\nJOIN [dvld].[dbo].[People] P ON P.PersonID = D.PersonID\r\nLEFT JOIN [dvld].[dbo].[Licenses] L \r\n    " +
                "ON D.DriverID = L.DriverID \r\n    AND L.IsActive = 1\r\nGROUP BY D.DriverID, D.PersonID, D.CreatedDate,\r\n" +
                "P.NationalNo, P.FirstName, P.SecondName, P.ThirdName, P.LastName\r\n\r\n) R1";
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

        public static int GetFilteredDriversCount(string filterkeyWord)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "select count(*) from " +
                "(\r\n\r\nSELECT \r\n    " +
                "D.[DriverID] AS 'Driver ID',\r\n    " +
                "D.[PersonID] AS 'Person ID',\r\n    " +
                "P.[Nationalno] AS 'National No.',\r\n    " +
                "concat(P.FirstName, ' ', P.SecondName, ' ', P.ThirdName, ' ', P.LastName) as 'Full Name',\r\n    " +
                "D.[CreatedDate] AS 'Date',\r\n    " +
                "COUNT(L.[LicenseID]) AS 'Active Licenses'\r\n" +
                "FROM [dvld].[dbo].[Drivers] D\r\nJOIN [dvld].[dbo].[People] P ON P.PersonID = D.PersonID\r\nLEFT JOIN [dvld].[dbo].[Licenses] L \r\n    " +
                "ON D.DriverID = L.DriverID \r\n    AND L.IsActive = 1\r\nGROUP BY D.DriverID, D.PersonID, D.CreatedDate,\r\n" +
                "P.NationalNo, P.FirstName, P.SecondName, P.ThirdName, P.LastName\r\n\r\n) R1" + GetFilterConditionText(filterkeyWord);
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

        public static bool DoesDriverExistWithSamePersonId(int personId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Drivers WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", personId);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
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

        public static int GetDriversIdByPersonId(int personId)
        {
            int driverId = -1;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n    " +
                "DriverId\r\n" +
                $"FROM [dvld].[dbo].[Drivers] D Where PersonID = {personId}";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    driverId = (int)(dt.Rows[0][0] ?? null);
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

            return driverId;
        }

        public static bool GetDriverDataById(int driverId, ref int personId, ref int createdByUserId, ref DateTime createdDate)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Drivers Where DriverId = @DriverId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DriverId", driverId);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    personId = (int)reader["PersonID"];
                    createdByUserId = (int)reader["CreatedByUserID"];
                    createdDate = (DateTime)reader["CreatedDate"];
                }
                else
                {
                    // The record was not found
                    isFound = false;
                }
                reader.Close();
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
    }
}
