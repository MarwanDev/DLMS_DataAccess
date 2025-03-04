using System;
using System.Data;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class PersonData
    {
        public static bool GetPersonInfoById(int id, ref string firstName, ref string secondName, ref string thirdName,
            ref string lastName, ref string nationalNo, ref DateTime dateOfBirth, ref byte gender, ref string email,
            ref string phone, ref string address, ref int nationalityCountryID, ref string imagePath, ref string country)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT *, Countries.CountryName as Country FROM People \n" +
                "join Countries on Countries.CountryID = People.NationalityCountryID \n" +
                "WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    firstName = (string)reader["FirstName"];
                    secondName = (string)reader["SecondName"];
                    thirdName = (string)reader["ThirdName"];
                    lastName = (string)reader["LastName"];
                    nationalNo = (string)reader["NationalNo"];
                    gender = (byte)reader["Gender"];
                    email = (string)reader["Email"];
                    phone = (string)reader["Phone"];
                    address = (string)reader["Address"];
                    dateOfBirth = (DateTime)reader["DateOfBirth"];
                    nationalityCountryID = (int)reader["NationalityCountryID"];
                    country = (string)reader["country"];

                    //imagePath: allows null in database so we should handle null
                    if (reader["ImagePath"] != DBNull.Value)
                    {
                        imagePath = (string)reader["ImagePath"];
                    }
                    else
                    {
                        imagePath = "";
                    }
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


        private static readonly string getPeopleCountQuery = "select Count(*) as count from (\n" +
            "SELECT [PersonID]\r\n      ,[NationalNo],[FirstName]\r\n  " +
                "    ,[SecondName]\r\n " +
                "     ,[ThirdName]\r\n      ,[LastName]\r\n      ,[DateOfBirth] as DOB\r\n  " +
                "    ,CASE\r\n\t\tWHEN Gender = 0 THEN 'Female'\r\n\t\tELSE 'Male'\r\n\t\tEND AS Gender\r\n    " +
                ",[Phone]\r\n      ,[Email]\r\n      ,[Countries].CountryName as Country\r\n   " +
                "FROM [dvld].[dbo].[People] join Countries\r\n " +
                " on Countries.CountryID = People.NationalityCountryID" + "\n  ) R1";
        private static readonly string getPeopleQuery = "select * from (" +
            "SELECT [PersonID]\r\n      ,[NationalNo],[FirstName]\r\n  " +
                "    ,[SecondName]\r\n " +
                "     ,[ThirdName]\r\n      ,[LastName]\r\n      ,[DateOfBirth] as DOB\r\n  " +
                "    ,CASE\r\n\t\tWHEN Gender = 0 THEN 'Female'\r\n\t\tELSE 'Male'\r\n\t\tEND AS Gender\r\n    " +
                ",[Phone]\r\n      ,[Email]\r\n      ,[Countries].CountryName as Country\r\n   " +
                "FROM [dvld].[dbo].[People] join Countries\r\n " +
                " on Countries.CountryID = People.NationalityCountryID" + "\n  ) R1";
        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = getPeopleQuery + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dt.Columns["FirstName"].ColumnName = "First Name";
                    dt.Columns["SecondName"].ColumnName = "Second Name";
                    dt.Columns["ThirdName"].ColumnName = "Third Name";
                    dt.Columns["LastName"].ColumnName = "Last Name";
                    dt.Columns["NationalNo"].ColumnName = "National No.";
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

        public enum FilterMode
        {
            PersonId,
            NationalNo,
            FirstName,
            SecondName,
            ThirdName,
            LastName,
            Country,
            Phone,
            Email,
            Gender,
            None
        };

        public static FilterMode CurrentFilterMode;

        public static void ApplySorting()
        {
            SortingCondition = IsSortingUsed ? GetSortingCommand() : "";
        }

        private static string GetFilterConditionText(string filterkeyWord)
        {
            string filterCondition = CurrentFilterMode == FilterMode.FirstName ||
                CurrentFilterMode == FilterMode.SecondName ||
                CurrentFilterMode == FilterMode.ThirdName ||
                CurrentFilterMode == FilterMode.LastName ||
                CurrentFilterMode == FilterMode.Country ||
                CurrentFilterMode == FilterMode.Phone ||
                CurrentFilterMode == FilterMode.NationalNo ||
                CurrentFilterMode == FilterMode.Email ||
                CurrentFilterMode == FilterMode.PersonId ?
                $"{CurrentFilterMode} like '%{filterkeyWord}%'" :
                $"{CurrentFilterMode} = '{filterkeyWord}'";
            return filterCondition.Length > 0 ? "\nwhere " + filterCondition : "";
        }

        public static DataTable FilterPeople(string filterkeyWord)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = getPeopleQuery + GetFilterConditionText(filterkeyWord) + SortingCondition;
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dt.Columns["FirstName"].ColumnName = "First Name";
                    dt.Columns["SecondName"].ColumnName = "Second Name";
                    dt.Columns["ThirdName"].ColumnName = "Third Name";
                    dt.Columns["LastName"].ColumnName = "Last Name";
                    dt.Columns["NationalNo"].ColumnName = "National No.";
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

        public static int AddNewPerson(string firstName, string secondName, string thirdName, string lastName, string nationalNo,
            DateTime dateOfBirth, byte gender, string email, string phone, string address,
            int nationalityCountryID, string imagePath)
        {
            int personID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO People (FirstName, SecondName, ThirdName, LastName, NationalNo, Gender, Email, 
                            Phone, Address, DateOfBirth, NationalityCountryID, ImagePath)
                            VALUES (@FirstName, @SecondName, @ThirdName, @LastName, @NationalNo, @Gender, @Email, @Phone, 
                            @Address, @DateOfBirth, @NationalityCountryID, @ImagePath);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", thirdName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@NationalNo", nationalNo);
            command.Parameters.AddWithValue("@Gender", gender);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Phone", phone);
            command.Parameters.AddWithValue("@Address", address);
            command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
            command.Parameters.AddWithValue("@NationalityCountryID", nationalityCountryID);

            if (!string.IsNullOrEmpty(imagePath))
                command.Parameters.AddWithValue("@ImagePath", imagePath);
            else
                command.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    personID = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return personID;
        }

        public static bool UpdatePerson(int id, string firstName, string secondName,
            string thirdName, string lastName, string nationalNo, DateTime dateOfBirth, byte gender,
            string email, string phone, string address, int nationalityCountryID, string imagePath)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  People  
                            set FirstName = @FirstName, 
                                SecondName = @SecondName, 
                                ThirdName = @ThirdName, 
                                LastName = @LastName, 
                                NationalNo = @NationalNo, 
                                Gender = @Gender, 
                                Email = @Email, 
                                Phone = @Phone, 
                                Address = @Address, 
                                DateOfBirth = @DateOfBirth,
                                NationalityCountryID = @NationalityCountryID,
                                ImagePath = @ImagePath
                                where PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", id);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", thirdName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@NationalNo", nationalNo);
            command.Parameters.AddWithValue("@Gender", gender);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Phone", phone);
            command.Parameters.AddWithValue("@Address", address);
            command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
            command.Parameters.AddWithValue("@NationalityCountryID", nationalityCountryID);

            if (!string.IsNullOrEmpty(imagePath))
                command.Parameters.AddWithValue("@ImagePath", imagePath);
            else
                command.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);

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

        public static bool DeletePerson(int personId)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Delete People 
                                where PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonId", personId);
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

        public static bool DoesPersonExist(int id)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", id);
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

        public static bool DoesPersonExist(string filterParam, string searchKeyWord)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = $"SELECT Found=1 FROM People WHERE {filterParam} = '{searchKeyWord}'";
            SqlCommand command = new SqlCommand(query, connection);
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

        public static int GetAllPeopleCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = getPeopleCountQuery;
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

        public static int GetFilteredPeopleCount(string filterkeyWord)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = getPeopleCountQuery + GetFilterConditionText(filterkeyWord);
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
