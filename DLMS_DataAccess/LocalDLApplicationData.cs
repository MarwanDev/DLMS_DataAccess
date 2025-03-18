using System;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class LocalDLApplicationData
    {
        public static int AddNewLocalDLApplication(int applicantPersonId,
            DateTime applicationDate, int applicationTypeId, int applicationStatus,
            DateTime lastStatusDate, decimal paidFees, int createdByUserId, int licenceClassId)
        {
            int applicationId = ApplicationData.AddNewApplication(applicantPersonId, applicationDate, applicationTypeId,
                applicationStatus, lastStatusDate, paidFees, createdByUserId);
            int localDLAPPicationId = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO LocalDrivingLicenseApplications (ApplicationID, LicenseClassID)
                            VALUES (@ApplicationID, @LicenseClassID);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);
            command.Parameters.AddWithValue("@LicenseClassID", licenceClassId);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    localDLAPPicationId = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return localDLAPPicationId;
        }
    }
}
