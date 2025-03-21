using System;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class ApplicationData
    {
        public static int AddNewApplication(int applicantPersonId,
            DateTime applicationDate, int applicationTypeId, int applicationStatus,
            DateTime lastStatusDate, decimal paidFees, int createdByUserId)
        {
            int applicationId = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Applications (ApplicantPersonID, ApplicationDate, ApplicationTypeID,
                                ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                            VALUES (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID,
                                @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicantPersonID", applicantPersonId);
            command.Parameters.AddWithValue("@ApplicationDate", applicationDate);
            command.Parameters.AddWithValue("@ApplicationTypeID", applicationTypeId);
            command.Parameters.AddWithValue("@ApplicationStatus", applicationStatus);
            command.Parameters.AddWithValue("@LastStatusDate", lastStatusDate);
            command.Parameters.AddWithValue("@PaidFees", paidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", createdByUserId);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    applicationId = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return applicationId;
        }
    }
}
