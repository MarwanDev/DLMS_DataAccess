using System;
using System.Data.SqlClient;

namespace DLMS_DataAccess.Test
{
    public class TestData
    {
        public static int TakeTest(int testAppointmentId, bool testResult, string notes, int createdByUserId)
        {
            int testID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Tests (TestAppointmentId, TestResult, Notes, CreatedByUserID)
                            VALUES (@TestAppointmentId, @TestResult, @Notes, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TestAppointmentId", testAppointmentId);
            command.Parameters.AddWithValue("@TestResult", testResult);
            command.Parameters.AddWithValue("@Notes", notes);
            command.Parameters.AddWithValue("@CreatedByUserID", createdByUserId);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    testID = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return testID;
        }
    }
}
