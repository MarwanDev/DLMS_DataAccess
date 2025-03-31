using System;
using System.Data.SqlClient;
using System.Security.Cryptography;

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

        public static bool GetLicenceDataById(int id, ref int driverId, ref DateTime issueDate, ref DateTime expirationDate,
            ref string notes, ref decimal paidFees, ref bool isActive, ref byte issueReason, ref string fullName,
            ref string className, ref string nationalNo, ref string gender, ref DateTime dateOfBirth, ref string imagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [LicenseID]\r\n      " +
                ",[DriverID]\r\n      " +
                ",[IssueDate]\r\n      " +
                ",[ExpirationDate]\r\n      " +
                ",[Notes]\r\n      " +
                ",Licenses.[PaidFees]\r\n      " +
                ",[IsActive]\r\n      " +
                ",[IssueReason]\r\n\t  " +
                ",LicenseClasses.ClassName\r\n\t  " +
                ",CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name'\r\n\t  " +
                ",People.NationalNo\r\n\t  " +
                ",CASE\r\n\t  " +
                "WHEN People.Gender = 0 " +
                "THEN 'Female'\r\n\t  " +
                "ELSE 'Male'\r\n\t  " +
                "END AS Gender\r\n\t  " +
                ",People.ImagePath\r\n\t  " +
                ",People.DateOfBirth\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "left join LicenseClasses on Licenses.LicenseClass = LicenseClasses.LicenseClassID\r\n  " +
                "left join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "left join People on People.PersonID = Applications.ApplicantPersonID\r\n  " +
                "where LicenseID = @LicenseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseID", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    driverId = (int)reader["DriverID"];
                    issueDate = (DateTime)reader["IssueDate"];
                    expirationDate = (DateTime)reader["ExpirationDate"];
                    paidFees = (decimal)reader["PaidFees"];
                    isActive = (bool)reader["IsActive"];
                    issueReason = (byte)reader["IssueReason"];
                    className = (string)reader["ClassName"];
                    fullName = (string)reader["Full Name"];
                    gender = (string)reader["Gender"];
                    dateOfBirth = (DateTime)reader["DateOfBirth"];
                    nationalNo = (string)reader["NationalNo"];

                    //notes: allows null in database so we should handle null
                    if (reader["Notes"] != DBNull.Value)
                    {
                        notes = (string)reader["Notes"];
                    }
                    else
                    {
                        notes = "";
                    }

                    //imagePath: allows null in database so we should handle null
                    if (reader["ImagePath"] != DBNull.Value)
                    {
                        imagePath = (string)reader["ImagePath"];
                    }
                    else
                    {
                        imagePath = "";
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

        public static bool GetLicenceDataByLocalDLApplicationId(int id, ref int driverId, ref DateTime issueDate, ref DateTime expirationDate,
            ref string notes, ref decimal paidFees, ref bool isActive, ref byte issueReason, ref string fullName,
            ref string className, ref string nationalNo, ref string gender, ref DateTime dateOfBirth, ref string imagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [LicenseID]\r\n      " +
                ",[DriverID]\r\n      " +
                ",[IssueDate]\r\n      " +
                ",[ExpirationDate]\r\n      " +
                ",[Notes]\r\n      " +
                ",Licenses.[PaidFees]\r\n      " +
                ",[IsActive]\r\n      " +
                ",[IssueReason]\r\n\t  " +
                ",LicenseClasses.ClassName\r\n\t  " +
                ",CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name'\r\n\t  " +
                ",People.NationalNo\r\n\t  " +
                ",People.ImagePath\r\n\t  " +
                ",CASE\r\n\t  " +
                "WHEN People.Gender = 0 " +
                "THEN 'Female'\r\n\t  " +
                "ELSE 'Male'\r\n\t  " +
                "END AS Gender\r\n\t  " +
                ",People.DateOfBirth\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "left join LicenseClasses on Licenses.LicenseClass = LicenseClasses.LicenseClassID\r\n  " +
                "left join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "left join LocalDrivingLicenseApplications on LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n  " +
                "left join People on People.PersonID = Applications.ApplicantPersonID\r\n  " +
                "where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    driverId = (int)reader["DriverID"];
                    issueDate = (DateTime)reader["IssueDate"];
                    expirationDate = (DateTime)reader["ExpirationDate"];
                    paidFees = (decimal)reader["PaidFees"];
                    isActive = (bool)reader["IsActive"];
                    issueReason = (byte)reader["IssueReason"];
                    className = (string)reader["ClassName"];
                    fullName = (string)reader["Full Name"];
                    gender = (string)reader["Gender"];
                    dateOfBirth = (DateTime)reader["DateOfBirth"];
                    nationalNo = (string)reader["NationalNo"];

                    //notes: allows null in database so we should handle null
                    if (reader["Notes"] != DBNull.Value)
                    {
                        notes = (string)reader["Notes"];
                    }
                    else
                    {
                        notes = "";
                    }

                    //imagePath: allows null in database so we should handle null
                    if (reader["ImagePath"] != DBNull.Value)
                    {
                        imagePath = (string)reader["ImagePath"];
                    }
                    else
                    {
                        imagePath = "";
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
