using System;
using System.Data.SqlClient;

namespace DLMS_DataAccess.Person
{
    public class DriverData
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
    }
}
