using System;
using System.Data;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class UserData
    {
        public static bool GetUserDataById(int id, ref string userName, ref string passwrod, ref bool isActive, ref int personId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [UserID]\r\n      ,[PersonID]\r\n      ,[UserName]\r\n      ,[Password]\r\n      ,[IsActive]\r\n " +
                " FROM [dvld].[dbo].[Users]\n" +
                "Where UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", id);

            try
            {
                // The record was found
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    userName = (string)reader["UserName"];
                    passwrod = (string)reader["Password"];
                    isActive = (bool)reader["IsActive"];
                    personId = (int)reader["PersonId"];
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

        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [UserID]\r\n" +
                ",[PersonID]\r\n" +
                ",[UserName]\r\n" +
                ",[IsActive]\r\n" +
                "FROM [dvld].[dbo].[Users]\n" + SortingCondition;
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
        private static string SortingCondition;
        public static void ApplySorting()
        {
            SortingCondition = IsSortingUsed ? GetSortingCommand() : "";
        }

        public enum FilterMode
        {
            PersonId,
            UserName,
            UserId,
            IsActive,
        };

        public static FilterMode CurrentFilterMode;

        public static int AddNewUser(int personId, string userName, string password, bool isActive)
        {
            int userID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Users (PersonID, UserName, Password, IsActive)
                            VALUES (@PersonID, @UserName, @Password, @IsActive);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", personId);
            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@IsActive", isActive);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    userID = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return userID;
        }

        public static bool UpdateUser(int id, int personId, string userName, string password, bool isActive)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  Users  
                            set UserName = @UserName, 
                                UserID = @UserID, 
                                PersonID = @PersonID, 
                                Password = @Password, 
                                IsActive = @IsActive
                                where UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", id);
            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@IsActive", isActive);
            command.Parameters.AddWithValue("@PersonID", personId);

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

        public static bool DeleteUser(int userId)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Delete Users 
                                where UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return (rowsAffected > 0);
        }

        public static bool DoesUserExist(int id)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", id);
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

        public static bool GetUserDataByAuthentication(string userName, string password, ref int id, ref bool isActive, ref int personId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [UserID]\r\n      ,[PersonID]\r\n      ,[UserName]\r\n      ,[Password]\r\n      ,[IsActive]\r\n " +
                " FROM [dvld].[dbo].[Users]\n" +
                "Where UserName = @UserName AND Password = @Password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@Password", password);

            try
            {
                // The record was found
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    id = (int)reader["UserID"];
                    userName = (string)reader["UserName"];
                    password = (string)reader["Password"];
                    isActive = (bool)reader["IsActive"];
                    personId = (int)reader["PersonId"];
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
    }
}
