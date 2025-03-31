using System;
using System.Data;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class LocalDLApplicationData : Data
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

        private static readonly string GetAllLocalApplicationsQuery = "SELECT * FROM (\r\n    " +
            "SELECT \r\n        " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID AS 'L.D.L.APP.ID',\r\n        " +
            "ClassName AS 'Driving Class',\r\n        " +
            "NationalNo AS 'National No.',\r\n        " +
            "CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name',\r\n        " +
            "[ApplicationDate] AS 'Application Date',\r\n\r\n        " +
            "(SELECT COUNT(*) \r\n         " +
            "FROM Tests \r\n         " +
            "JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n         " +
            "WHERE TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n           " +
            "AND Tests.TestResult = 1) AS 'Passed Tests',\r\n\r\n        " +
            "CASE\r\n            " +
            "WHEN [ApplicationStatus] = 0 THEN 'Cancelled'\r\n            " +
            "WHEN [ApplicationStatus] = 1 THEN 'New'\r\n            " +
            "WHEN [ApplicationStatus] = 2 THEN 'In Progress'\r\n            " +
            "WHEN [ApplicationStatus] = 3 THEN 'Complete'\r\n        " +
            "END AS 'Status'\r\n        \r\n    " +
            "FROM LocalDrivingLicenseApplications \r\n    " +
            "JOIN [dvld].[dbo].[Applications] \r\n        " +
            "ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n    " +
            "JOIN People \r\n        ON People.PersonID = Applications.ApplicantPersonID\r\n    " +
            "LEFT JOIN LicenseClasses \r\n        " +
            "ON LicenseClasses.LicenseClassID = LocalDrivingLicenseApplications.LicenseClassID\r\n    " +
            "LEFT JOIN TestAppointments \r\n        " +
            "ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n    " +
            "LEFT JOIN Tests \r\n        " +
            "ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n\t\tGROUP BY \r\n    " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,\r\n    " +
            "ClassName,\r\n    " +
            "NationalNo,\r\n    " +
            "People.FirstName, " +
            "People.SecondName, " +
            "People.ThirdName, " +
            "People.LastName,\r\n    " +
            "ApplicationDate,\r\n    " +
            "ApplicationStatus\r\n) r1";

        private static readonly string GetAllLocalApplicationsCountQuery = "SELECT Count (*) FROM (\r\n    " +
            "SELECT \r\n        " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID AS 'L.D.L.APP.ID',\r\n        " +
            "ClassName AS 'Driving Class',\r\n        " +
            "NationalNo AS 'National No.',\r\n        " +
            "CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name',\r\n        " +
            "[ApplicationDate] AS 'Application Date',\r\n\r\n        " +
            "(SELECT COUNT(*) \r\n         " +
            "FROM Tests \r\n         " +
            "JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n         " +
            "WHERE TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n           " +
            "AND Tests.TestResult = 1) AS 'Passed Tests',\r\n\r\n        " +
            "CASE\r\n            " +
            "WHEN [ApplicationStatus] = 0 THEN 'Cancelled'\r\n            " +
            "WHEN [ApplicationStatus] = 1 THEN 'New'\r\n            " +
            "WHEN [ApplicationStatus] = 2 THEN 'In Progress'\r\n            " +
            "WHEN [ApplicationStatus] = 3 THEN 'Complete'\r\n        " +
            "END AS 'Status'\r\n        \r\n    " +
            "FROM LocalDrivingLicenseApplications \r\n    " +
            "JOIN [dvld].[dbo].[Applications] \r\n        " +
            "ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n    " +
            "JOIN People \r\n        ON People.PersonID = Applications.ApplicantPersonID\r\n    " +
            "LEFT JOIN LicenseClasses \r\n        " +
            "ON LicenseClasses.LicenseClassID = LocalDrivingLicenseApplications.LicenseClassID\r\n    " +
            "LEFT JOIN TestAppointments \r\n        " +
            "ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n    " +
            "LEFT JOIN Tests \r\n        " +
            "ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n\t\tGROUP BY \r\n    " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,\r\n    " +
            "ClassName,\r\n    " +
            "NationalNo,\r\n    " +
            "People.FirstName, " +
            "People.SecondName, " +
            "People.ThirdName, " +
            "People.LastName,\r\n    " +
            "ApplicationDate,\r\n    " +
            "ApplicationStatus\r\n) r1";

        public static DataTable GetAllLocalDLApplications()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllLocalApplicationsQuery + SortingCondition;
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

        public static string CurrentFilter;

        private static string GetFilterConditionText(string filterkeyWord)
        {
            string filterCondition = CurrentFilter == "L.D.L.APP.ID" ||
                CurrentFilter == "National No." || CurrentFilter == "Full Name" ?
                $"\"{CurrentFilter}\" like '%{filterkeyWord}%'" :
                $"\"{CurrentFilter}\" = '{filterkeyWord}'";
            return filterCondition.Length > 0 ? "\nwhere " + filterCondition : "";
        }

        public static DataTable GetFilteredLocalDLApplications(string filterkeyWord)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllLocalApplicationsQuery + GetFilterConditionText(filterkeyWord) + SortingCondition;
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

        public static int GetAllLocalDLApplicationsCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllLocalApplicationsCountQuery;
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

        public static int GetFilteredLocalDLApplicationsCount(string filterkeyWord)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = GetAllLocalApplicationsCountQuery + GetFilterConditionText(filterkeyWord);
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

        public static bool CancelLocalDLApplication(int localDLApplicationId)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE
                                A
                            SET
                                A.ApplicationStatus = 0
                            FROM
                                Applications AS A
                                INNER JOIN LocalDrivingLicenseApplications AS L
                                ON L.ApplicationID = A.ApplicationID
                            WHERE
                                L.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDLApplicationId);

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

        public static bool UpdateLocalDLApplication(int id, int licenceClassId)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  LocalDrivingLicenseApplications  
                            set LicenseClassID = @LicenseClassID    
                                where LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", id);
            command.Parameters.AddWithValue("@LicenseClassID", licenceClassId);

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

        public static bool GetLocalDLApplicationInfoById(int id, ref int applicantPersonID, ref DateTime applicationDate,
            ref byte applicationStatus, ref decimal paidFees, ref string createdByUserName, ref int licenceClassID, ref string licenceClassName)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "select \r\n" +
                "LocalDrivingLicenseApplicationID,\r\n" +
                "ApplicantPersonID,\r\n" +
                //"Applications.ApplicationTypeID,\r\n" +
                "ApplicationStatus,\r\n" +
                //"LastStatusDate,\r\n" +
                "ApplicationDate,\r\n" +
                "PaidFees,\r\n" +
                //"CreatedByUserID,\r\n" +
                "LocalDrivingLicenseApplications.LicenseClassID,\r\n" +
                //"CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name',\r\n" +
                //"ApplicationTypeTitle,\r\n" +
                "UserName,\r\n" +
                "ClassName\r\n\r\n" +
                "from LocalDrivingLicenseApplications join Applications \r\n" +
                "on Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID join ApplicationTypes on\r\n" +
                "Applications.ApplicationTypeID = ApplicationTypes.ApplicationTypeID join People on\r\n" +
                "People.PersonID = Applications.ApplicantPersonID join Users on\r\n" +
                "Users.UserID = Applications.CreatedByUserID join LicenseClasses on\r\n" +
                "LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID\r\n\r\n" +
                "where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDLApplicationId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDLApplicationId", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    applicantPersonID = (int)reader["ApplicantPersonID"];
                    applicationStatus = (byte)reader["ApplicationStatus"];
                    applicationDate = (DateTime)reader["ApplicationDate"];
                    paidFees = (decimal)reader["PaidFees"];
                    createdByUserName = (string)reader["UserName"];
                    licenceClassID = (int)reader["LicenseClassID"];
                    licenceClassName = (string)reader["ClassName"];
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

        public static bool GetLocalDLApplicationMoreDetailsById(int id, ref string className, ref string fullName, 
            ref DateTime applicationDate, ref int passedTests, ref string statusText, ref int applicationId, 
            ref decimal paidFees, ref string applicationTypeTitle, ref DateTime lastStatusDate, ref string userName)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM (\r\n    SELECT \r\n        " +
                "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID AS 'L.D.L.APP.ID',\r\n        " +
                "ClassName AS 'Driving Class',\r\n        " +
                "CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name',\r\n        " +
                "[ApplicationDate] AS 'Application Date',\r\n\r\n        " +
                "(SELECT COUNT(*) \r\n         " +
                "FROM Tests \r\n         " +
                "JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n         " +
                "WHERE TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n           " +
                "AND Tests.TestResult = 1) AS 'Passed Tests',\r\n\r\n        " +
                "CASE\r\n            " +
                "WHEN [ApplicationStatus] = 0 THEN 'Cancelled'\r\n            " +
                "WHEN [ApplicationStatus] = 1 THEN 'New'\r\n            " +
                "WHEN [ApplicationStatus] = 2 THEN 'In Progress'\r\n            " +
                "WHEN [ApplicationStatus] = 3 THEN 'Complete'\r\n        " +
                "END AS 'Status',\r\n\t\t\r\n\t\tLocalDrivingLicenseApplications.ApplicationID,\r\n\t\t" +
                "Applications.PaidFees,\r\n\t\tApplicationTypeTitle,\r\n\t\tLastStatusDate,\r\n\t\tUserName\r\n        \r\n    " +
                "FROM LocalDrivingLicenseApplications \r\n    " +
                "JOIN [dvld].[dbo].[Applications] \r\n        " +
                "ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n    " +
                "JOIN People \r\n        ON People.PersonID = Applications.ApplicantPersonID\r\n    " +
                "JOIN Users \r\n        ON Users.UserID = Applications.CreatedByUserID\r\n    " +
                "JOIN ApplicationTypes \r\n        " +
                "ON ApplicationTypes.ApplicationTypeID = Applications.ApplicationTypeID\r\n    " +
                "LEFT JOIN LicenseClasses \r\n        " +
                "ON LicenseClasses.LicenseClassID = LocalDrivingLicenseApplications.LicenseClassID\r\n    " +
                "LEFT JOIN TestAppointments \r\n        " +
                "ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n    " +
                "LEFT JOIN Tests \r\n        " +
                "ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n\t\tGROUP BY \r\n    " +
                "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,\r\n    " +
                "LocalDrivingLicenseApplications.ApplicationID," +
                "\r\n\tApplications.PaidFees," +
                "\r\n\tApplicationTypes.ApplicationTypeTitle,\r\n    " +
                "People.FirstName, " +
                "People.SecondName, " +
                "People.ThirdName, " +
                "People.LastName,\r\n    " +
                "ApplicationDate,\r\n    " +
                "LastStatusDate,\r\n    " +
                "ApplicationStatus," +
                "ClassName," +
                "\r\n\tUserName\r\n) r1\r\n" +
                "where \"L.D.L.APP.ID\" = @LocalDLApplicationID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDLApplicationID", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    className = (string)reader["Driving Class"];
                    fullName = (string)reader["Full Name"];
                    applicationDate = (DateTime)reader["Application Date"];
                    lastStatusDate = (DateTime)reader["LastStatusDate"];
                    paidFees = (decimal)reader["PaidFees"];
                    passedTests = (int)reader["Passed Tests"];
                    statusText = (string)reader["Status"];
                    applicationId = (int)reader["ApplicationID"];
                    userName = (string)reader["UserName"];
                    applicationTypeTitle = (string)reader["ApplicationTypeTitle"];
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

        public static int GetPassedTestsCountById(int id)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n\r\n        " +
            "(SELECT COUNT(*) \r\n         " +
            "FROM Tests \r\n         " +
            "JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n         " +
            "WHERE TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n           " +
            "AND Tests.TestResult = 1) AS 'Passed Tests'\r\n        \r\n    " +
            "FROM LocalDrivingLicenseApplications \r\n    " +
            "JOIN [dvld].[dbo].[Applications] \r\n        " +
            "ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n    " +
            "JOIN People \r\n        ON People.PersonID = Applications.ApplicantPersonID\r\n    " +
            "LEFT JOIN LicenseClasses \r\n        ON LicenseClasses.LicenseClassID = LocalDrivingLicenseApplications.LicenseClassID\r\n    " +
            "LEFT JOIN TestAppointments \r\n        ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n    " +
            "LEFT JOIN Tests \r\n        " +
            "ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n\t\t" +
            $"where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = {id}" +
            "\r\n\t\tGROUP BY \r\n    " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,\r\n    " +
            "ClassName,\r\n    " +
            "NationalNo,\r\n    " +
            "People.FirstName, People.SecondName, People.ThirdName, People.LastName,\r\n    " +
            "ApplicationDate,\r\n" +
            "ApplicationStatus";
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

        public static byte GetApplicationStatusById(int id)
        {
            byte status = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n\r\n        " +
            "ApplicationStatus \r\n         " +
            "FROM LocalDrivingLicenseApplications \r\n    " +
            "JOIN [dvld].[dbo].[Applications] \r\n        " +
            "ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n    " +
            "JOIN People \r\n        ON People.PersonID = Applications.ApplicantPersonID\r\n    " +
            "LEFT JOIN LicenseClasses \r\n        ON LicenseClasses.LicenseClassID = LocalDrivingLicenseApplications.LicenseClassID\r\n    " +
            "LEFT JOIN TestAppointments \r\n        ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n    " +
            "LEFT JOIN Tests \r\n        " +
            "ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n\t\t" +
            $"where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = {id}" +
            "\r\n\t\tGROUP BY \r\n    " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,\r\n    " +
            "ClassName,\r\n    " +
            "NationalNo,\r\n    " +
            "People.FirstName, People.SecondName, People.ThirdName, People.LastName,\r\n    " +
            "ApplicationDate,\r\n" +
            "ApplicationStatus";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    status = (byte)(dt.Rows[0][0] ?? null);
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

            return status;
        }

        public static int GetApplicationId(int id)
        {
            int applicationId = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n\r\n        " +
            "LocalDrivingLicenseApplications.ApplicationID        \r\n    " +
            "FROM LocalDrivingLicenseApplications \r\n    " +
            "JOIN [dvld].[dbo].[Applications] \r\n        " +
            "ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n    " +
            "JOIN People \r\n        ON People.PersonID = Applications.ApplicantPersonID\r\n    " +
            "LEFT JOIN LicenseClasses \r\n        ON LicenseClasses.LicenseClassID = LocalDrivingLicenseApplications.LicenseClassID\r\n    " +
            "LEFT JOIN TestAppointments \r\n        ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID\r\n    " +
            "LEFT JOIN Tests \r\n        " +
            "ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID\r\n\t\t" +
            $"where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = {id}" +
            "\r\n\t\tGROUP BY \r\n    " +
            "LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,\r\n    " +
            "LocalDrivingLicenseApplications.ApplicationID,\r\n    " +
            "ClassName,\r\n    " +
            "NationalNo,\r\n    " +
            "People.FirstName, People.SecondName, People.ThirdName, People.LastName,\r\n    " +
            "ApplicationDate,\r\n" +
            "ApplicationStatus";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    applicationId = (int)(dt.Rows[0][0] ?? null);
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

            return applicationId;
        }

        public static int GetLicenceClassId(int id)
        {
            int applicationId = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n\r\n        " +
            "LicenseClassID        \r\n    " +
            "FROM LocalDrivingLicenseApplications \r\n    " +
            $"where LocalDrivingLicenseApplicationID = {id}";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    applicationId = (int)(dt.Rows[0][0] ?? null);
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

            return applicationId;
        }

        public static int GetPersonApplicantId(int id)
        {
            int applicantId = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n\t[ApplicantPersonID]\r\n  " +
                "FROM [dvld].[dbo].[Applications]\r\n  " +
                "join LocalDrivingLicenseApplications on \r\n  " +
                "LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n  " +
                $"where LocalDrivingLicenseApplicationID = {id}\r\n";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    applicantId = (int)(dt.Rows[0][0] ?? null);
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

            return applicantId;
        }

        public static bool DeleteLocalDLApplication(int id)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Delete LocalDrivingLicenseApplications 
                                where LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", id);
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

        public static bool ChangeApplicationStatus(int id, byte status)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE
                                A
                            SET
                                A.ApplicationStatus = @ApplicationStatus
                            FROM
                                Applications AS A
                                INNER JOIN LocalDrivingLicenseApplications AS L
                                ON L.ApplicationID = A.ApplicationID
                            WHERE
                                L.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", id);
            command.Parameters.AddWithValue("@ApplicationStatus", status);

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
    }
}
