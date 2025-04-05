
using System.Data.SqlClient;
using System;
using System.Data;

namespace DLMS_DataAccess.Licence
{
    public class DetainedLicenceData : Data
    {
        public static bool IsLicenceDetained(int licenceId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM DetainedLicenses WHERE LicenseID = @LicenseID AND IsReleased = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseID", licenceId);
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

        public static bool ReleaseDetainedLicence(int id, DateTime releaseDate, int releasedByUserId, int releaseApplicationId)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  DetainedLicenses  
                            set ReleaseDate = @ReleaseDate, 
                                ReleasedByUserID = @ReleasedByUserID, 
                                ReleaseApplicationID = @ReleaseApplicationID,
                                IsReleased = 1
                                where DetainID = @DetainID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DetainID", id);
            command.Parameters.AddWithValue("@ReleaseDate", releaseDate);
            command.Parameters.AddWithValue("@ReleasedByUserID", releasedByUserId);
            command.Parameters.AddWithValue("@ReleaseApplicationID", releaseApplicationId);

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

        public static bool GetDetainedLicenceDatById(int id, ref int licenceId, ref DateTime detainDate, ref decimal fineFees,
            ref int createdByUserId, ref bool isReleased, ref DateTime releaseDate, ref int releasedByUserId, ref int releaseApplicationId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM DetainedLicence \n" +
                "WHERE DetainID = @DetainID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DetainID", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    licenceId = (int)reader["LicenseID"];
                    detainDate = (DateTime)reader["DetainDate"];
                    fineFees = (decimal)reader["FineFees"];
                    createdByUserId = (int)reader["CreatedByUserID"];
                    isReleased = (bool)reader["IsReleased"];

                    ////imagePath: allows null in database so we should handle null
                    if (reader["ReleaseDate"] != DBNull.Value)
                    {
                        releaseDate = (DateTime)reader["ReleaseDate"];
                    }
                    else
                    {
                        releaseDate = new DateTime();
                    }

                    if (reader["ReleasedByUserID"] != DBNull.Value)
                    {
                        releasedByUserId = (int)reader["ReleasedByUserID"];
                    }
                    else
                    {
                        releasedByUserId = -1;
                    }

                    if (reader["ReleaseApplicationID"] != DBNull.Value)
                    {
                        releaseApplicationId = (int)reader["ReleaseApplicationID"];
                    }
                    else
                    {
                        releaseApplicationId = -1;
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

        public static bool GetDetainedLicenceDatByLicenceId(ref int id, int licenceId, ref DateTime detainDate, ref decimal fineFees,
            ref int createdByUserId, ref bool isReleased, ref DateTime releaseDate, ref int releasedByUserId, ref int releaseApplicationId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM DetainedLicenses \n" +
                "WHERE LicenseID = @LicenseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseID", licenceId);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    id = (int)reader["DetainID"];
                    detainDate = (DateTime)reader["DetainDate"];
                    fineFees = (decimal)reader["FineFees"];
                    createdByUserId = (int)reader["CreatedByUserID"];
                    isReleased = (bool)reader["IsReleased"];

                    ////imagePath: allows null in database so we should handle null
                    if (reader["ReleaseDate"] != DBNull.Value)
                    {
                        releaseDate = (DateTime)reader["ReleaseDate"];
                    }
                    else
                    {
                        releaseDate = new DateTime();
                    }

                    if (reader["ReleasedByUserID"] != DBNull.Value)
                    {
                        releasedByUserId = (int)reader["ReleasedByUserID"];
                    }
                    else
                    {
                        releasedByUserId = -1;
                    }

                    if (reader["ReleaseApplicationID"] != DBNull.Value)
                    {
                        releaseApplicationId = (int)reader["ReleaseApplicationID"];
                    }
                    else
                    {
                        releaseApplicationId = -1;
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

        private static readonly string GetAllDetainedLicencesQuery = "Select * from (" +
            "\r\n\r\nSELECT [DetainID] as 'D.ID'\r\n      " +
            ",DetainedLicenses.LicenseID as 'L.ID'\r\n      " +
            ",[DetainDate] as 'D.Date'\r\n      " +
            ",[FineFees] as 'Fine Fees'\r\n      " +
            ",[IsReleased] as 'Is Released'\r\n      " +
            ",[ReleaseDate] as 'Release Date'\r\n      " +
            ",[NationalNo] as 'N.No'\r\n      " +
            ",CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name'\r\n      " +
            ",[ReleaseApplicationID] as 'Release App.ID'\r\n  " +
            "FROM [dvld].[dbo].[DetainedLicenses]\r\n  " +
            "join Licenses on DetainedLicenses.LicenseID = Licenses.LicenseID\r\n  " +
            "join Drivers on Drivers.DriverID = Licenses.DriverID\r\n  " +
            "join People on People.PersonID = Drivers.PersonID\r\n  " +
            ")\r\n  R1\n";

        private static readonly string GetAllDetainedLicencesCountQuery = "Select Count (*) from (" +
            "\r\n\r\nSELECT [DetainID] as 'D.ID'\r\n      " +
            ",DetainedLicenses.LicenseID as 'L.ID'\r\n      " +
            ",[DetainDate] as 'D.Date'\r\n      " +
            ",[FineFees] as 'Fine Fees'\r\n      " +
            ",[IsReleased] as 'Is Released'\r\n      " +
            ",[ReleaseDate] as 'Release Date'\r\n      " +
            ",[NationalNo] as 'N.No'\r\n      " +
            ",CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name'\r\n      " +
            ",[ReleaseApplicationID] as 'Release App.ID'\r\n  " +
            "FROM [dvld].[dbo].[DetainedLicenses]\r\n  " +
            "join Licenses on DetainedLicenses.LicenseID = Licenses.LicenseID\r\n  " +
            "join Drivers on Drivers.DriverID = Licenses.DriverID\r\n  " +
            "join People on People.PersonID = Drivers.PersonID\r\n  " +
            ")\r\n  R1\n";

        public static DataTable GetAllDetainedLicences()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllDetainedLicencesQuery + SortingCondition;
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

        public static int GetAllDetainedLicencesCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllDetainedLicencesCountQuery;
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

        public static int GetFilteredDetainedLicencesCount(string filterkeyWord)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllDetainedLicencesCountQuery + GetFilterConditionText(filterkeyWord);
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

        public static string CurrentFilter;

        private static string GetFilterConditionText(string filterkeyWord)
        {
            string filterCondition = CurrentFilter != "Is Released" ?
                $"\"{CurrentFilter}\" like '%{filterkeyWord}%'" :
                $"\"{CurrentFilter}\" = '{filterkeyWord}'";
            return filterCondition.Length > 0 ? "\nwhere " + filterCondition : "";
        }

        public static DataTable GetFilteredDetainedLicences(string filterkeyWord)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllDetainedLicencesQuery + GetFilterConditionText(filterkeyWord) + SortingCondition;
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
    }
}
