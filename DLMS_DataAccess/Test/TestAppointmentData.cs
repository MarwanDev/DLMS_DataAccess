using System;
using System.Data;
using System.Data.SqlClient;

namespace DLMS_DataAccess
{
    public class TestAppointmentData : Data
    {
        public static int AddNewTestAppointment(int testTypeId, int localDLApplicationId, DateTime appointmentDate, 
            decimal paidFees, int createdByUserId, bool isLocked)
        {
            int testAppointmentID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO TestAppointments (TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked)
                            VALUES (@TestTypeID, @LocalDrivingLicenseApplicationID, @AppointmentDate, @PaidFees, @CreatedByUserID, @IsLocked);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TestTypeID", testTypeId);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDLApplicationId);
            command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
            command.Parameters.AddWithValue("@PaidFees", paidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", createdByUserId);
            command.Parameters.AddWithValue("@IsLocked", isLocked);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    testAppointmentID = insertedID;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return testAppointmentID;
        }

        public static DataTable GetAllTestAppointmentsForLocalDLApplication(int id)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [TestAppointmentID] AS 'Appointment ID'\r\n      " +
                ",[AppointmentDate] AS 'Appointment Date'\r\n     " +
                ",[PaidFees] AS 'Paid Fees'\r\n      " +
                ",[IsLocked] AS 'Is Locked'\r\n  " +
                "FROM [dvld].[dbo].[TestAppointments]\r\n" +
                $"WHERE LocalDrivingLicenseApplicationID = {id}";
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

        public static int GetAllTestAppointmentsCountForLocalDLApplication(int id)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Count(*) AS Count\r\n      " +
                "FROM [dvld].[dbo].[TestAppointments]\r\n" +
                $"WHERE LocalDrivingLicenseApplicationID = {id}";
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
