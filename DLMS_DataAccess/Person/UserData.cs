using System;
using System.Data;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class UserData : Data
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

        public static bool GetUserDataByPersonId(ref int id, ref string userName, ref string passwrod, ref bool isActive, int personId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [UserID]\r\n      ,[PersonID]\r\n      ,[UserName]\r\n      ,[Password]\r\n      ,[IsActive]\r\n " +
                " FROM [dvld].[dbo].[Users]\n" +
                "Where PersonId = @PersonId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonId", personId);

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
                    id = (int)reader["userId"];
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
                ",Users.PersonID\r\n    " +
                ",concat(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) as FullName" +
                ",[UserName]\r\n" +
                ",[IsActive]\r\n" +
                "FROM [dvld].[dbo].[Users] \n" +
                "join People on \n" +
                "People.PersonID = Users.PersonID" + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dt.Columns["FullName"].ColumnName = "Full Name";
                    dt.Columns["IsActive"].ColumnName = "Is Active";
                    dt.Columns["UserID"].ColumnName = "User ID";
                    dt.Columns["PersonID"].ColumnName = "Person ID";
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

        private static string GetFilterConditionText(string filterkeyWord)
        {
            string filterCondition = CurrentFilterMode == FilterMode.PersonId ||
                CurrentFilterMode == FilterMode.FullName ||
                CurrentFilterMode == FilterMode.UserId ||
                CurrentFilterMode == FilterMode.UserName ?
                $"{CurrentFilterMode} like '%{filterkeyWord}%'" :
                $"{CurrentFilterMode} = '{filterkeyWord}'";
            return filterCondition.Length > 0 ? "\nwhere " + filterCondition : "";
        }

        public static DataTable GetFilteredUsers(string filterkeyWord)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query =
                "select * from (\r\n\r\n\r\n\tSELECT [UserID]\r\n\t\t  " +
                ",Users.PersonID\r\n\t\t  " +
                ",concat(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) as FullName\r\n\t\t " +
                " ,[UserName]\r\n\t\t  " +
                ",[IsActive]\r\n\t  " +
                "FROM [dvld].[dbo].[Users]\r\n\t  " +
                "join people on people.PersonID = Users.PersonID\r\n\t  ) " +
                "R1 \n "
                + GetFilterConditionText(filterkeyWord) + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dt.Columns["FullName"].ColumnName = "Full Name";
                    dt.Columns["IsActive"].ColumnName = "Is Active";
                    dt.Columns["UserID"].ColumnName = "User ID";
                    dt.Columns["PersonID"].ColumnName = "Person ID";
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

        public enum FilterMode
        {
            PersonId,
            FullName,
            UserName,
            UserId,
            IsActive,
        };

        public static void DisableSorting()
        {
            UserData.IsSortingUsed = false;
        }

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

        public static bool ChangeUserPassword(int id, string password)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  Users  
                            set Password = @Password
                                where UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", id);
            command.Parameters.AddWithValue("@Password", password);

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

        public static string GetUserPassword(int userId)
        {
            string password = "";
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = $"Select Password from Users\n Where UserId = {userId}";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    password = (string)(dt.Rows[0][0] ?? null);
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

            return password;
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

        private static readonly string GetUsersCountQuery = "select Count (*) as Count from (\r\n\r\n\r\n\t" +
            "SELECT [UserID]\r\n\t\t  ," +
            "Users.PersonID\r\n\t\t  ," +
            "concat(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) as FullName\r\n\t\t  ," +
            "[UserName]\r\n\t\t  ," +
            "[IsActive]\r\n\t  " +
            "FROM [dvld].[dbo].[Users]\r\n\t  " +
            "join people on people.PersonID = Users.PersonID\r\n\t  ) R1";

        public static int GetAllUsersCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetUsersCountQuery;
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

        public static int GetFilteredUsersCount(string filterkeyWord)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetUsersCountQuery + GetFilterConditionText(filterkeyWord);
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
    }
}
