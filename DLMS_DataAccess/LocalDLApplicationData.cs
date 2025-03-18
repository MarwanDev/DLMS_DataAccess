using System;
using System.Data;
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

        public static int GetApplicationIdForSamePersonAndLicenceClass(int licenceClassId, int applicantPersonId)
        {
            int localDLApplicationId = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = $"SELECT top 1 LocalDrivingLicenseApplicationID\r\n  " +
                $"FROM [dvld].[dbo].[Applications] join\r\n  " +
                $"[dbo].[LocalDrivingLicenseApplications] on " +
                $"[Applications].[ApplicationID] = [LocalDrivingLicenseApplications].[ApplicationID]\r\n  " +
                $"where [LocalDrivingLicenseApplications].LicenseClassID = {licenceClassId} and\r\n  " +
                $"ApplicationStatus <> 0 and\r\n  " +
                $"ApplicantPersonID = {applicantPersonId}";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    localDLApplicationId = (int)(dt.Rows[0][0] ?? null);
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

            return localDLApplicationId;
        }
    }
}
