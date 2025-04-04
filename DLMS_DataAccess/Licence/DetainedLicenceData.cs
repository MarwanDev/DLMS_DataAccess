
using System.Data.SqlClient;
using System;
using System.Net;
using System.Security.Policy;

namespace DLMS_DataAccess.Licence
{
    public class DetainedLicenceData
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
    }
}
