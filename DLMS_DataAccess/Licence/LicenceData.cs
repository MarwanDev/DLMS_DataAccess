using System;
using System.Data.SqlClient;

namespace DLMS_DataAccess.Licence
{
    public class LicenceData
    {
        public static int AddNewLicence(int applicationId, int driverId, int licenceClassId,
            DateTime issueDate, bool isActive, string notes,
            DateTime expirationDate, decimal paidFees, int createdByUserId, byte issueReason)
        {
            int licenceId = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Licenses (ApplicationID, DriverId, LicenseClass, IssueDate,
                            IsActive, Notes, ExpirationDate, PaidFees, CreatedByUserId, IssueReason)
                            VALUES (@ApplicationID, @DriverId, @LicenseClass, @IssueDate,
                            @IsActive, @Notes, @ExpirationDate, @PaidFees, @CreatedByUserId, @IssueReason);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);
            command.Parameters.AddWithValue("@DriverId", driverId);
            command.Parameters.AddWithValue("@LicenseClass", licenceClassId);
            command.Parameters.AddWithValue("@IssueDate", issueDate);
            command.Parameters.AddWithValue("@IsActive", isActive);
            command.Parameters.AddWithValue("@Notes", notes);
            command.Parameters.AddWithValue("@ExpirationDate", expirationDate);
            command.Parameters.AddWithValue("@PaidFees", paidFees);
            command.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);
            command.Parameters.AddWithValue("@IssueReason", issueReason);

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
