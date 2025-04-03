using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace DLMS_DataAccess.Licence
{
    public class LicenceData : Data
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

        public static int AddNewInternationalLicence(int applicationId, int driverId, int localLicenceId,
            DateTime issueDate, bool isActive, DateTime expirationDate, int createdByUserId)
        {
            int licenceId = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO InternationalLicenses (ApplicationID, DriverId, IssuedUsingLocalLicenseID, IssueDate,
                            IsActive, ExpirationDate, CreatedByUserId)
                            VALUES (@ApplicationID, @DriverId, @LocalLicenceId, @IssueDate,
                            @IsActive, @ExpirationDate, @CreatedByUserId);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);
            command.Parameters.AddWithValue("@DriverId", driverId);
            command.Parameters.AddWithValue("@LocalLicenceId", localLicenceId);
            command.Parameters.AddWithValue("@IssueDate", issueDate);
            command.Parameters.AddWithValue("@IsActive", isActive);
            command.Parameters.AddWithValue("@ExpirationDate", expirationDate);
            command.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);

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

        public static DataTable GetAllLocalLicencesPerPerson(int personId)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [LicenseID] as 'Lic. ID'\r\n      " +
                ",Licenses.[ApplicationID] as 'App. ID'\r\n      " +
                ",ClassName as 'Class Name'\r\n      " +
                ",[IssueDate] as 'Issue Date'\r\n      " +
                ",[ExpirationDate] as 'Expiration Date'\r\n      " +
                ",[IsActive] as 'Is Active'\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "left join LocalDrivingLicenseApplications on LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n  " +
                $"join People on People.PersonID = Applications.ApplicantPersonID\r\n  where PersonID = {personId}";
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

        public static DataTable GetAllInternationalLicencesPerPerson(int personId)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT  InternationalLicenses.InternationalLicenseID AS 'Int. licence ID'\r\n  " +
                ",Licenses.[ApplicationID] as 'Application ID'\r\n\t\t" +
                ",[IssuedUsingLocalLicenseID] as 'L.Licence ID'\r\n\t\t" +
                ",InternationalLicenses.IssueDate as 'Issue Date'\r\n\t\t" +
                ",InternationalLicenses.ExpirationDate as 'Expiration Date'\r\n\t\t" +
                ",InternationalLicenses.IsActive as 'Is Active'\r\n\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join InternationalLicenses on InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID\r\n  " +
                $"join People on People.PersonID = Applications.ApplicantPersonID\r\n  where PersonID = {personId}";
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

        public static DataTable GetAllInternationalLicences()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT  InternationalLicenses.InternationalLicenseID AS 'Int. licence ID'\r\n  " +
                ",Licenses.[ApplicationID] as 'Application ID'\r\n\t\t" +
                ",[IssuedUsingLocalLicenseID] as 'L.Licence ID'\r\n\t\t" +
                ",InternationalLicenses.IssueDate as 'Issue Date'\r\n\t\t" +
                ",InternationalLicenses.ExpirationDate as 'Expiration Date'\r\n\t\t" +
                ",InternationalLicenses.IsActive as 'Is Active'\r\n\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join InternationalLicenses on InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID\r\n  " +
                $"join People on People.PersonID = Applications.ApplicantPersonID\r\n" + SortingCondition;
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

        public static int GetAllLocalLicencePerPersonCount(int personId)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Count (*) as Count\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join LocalDrivingLicenseApplications on LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID\r\n  " +
                $"join People on People.PersonID = Applications.ApplicantPersonID\r\n  where PersonID = {personId}";
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

        public static int GetAllInternationalLicencePerPersonCount(int personId)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Count (*) as Count\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join InternationalLicenses on InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID\r\n  " +
                $"join People on People.PersonID = Applications.ApplicantPersonID\r\n  where PersonID = {personId}";
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

        public static int GetAllInternationalLicenceCount()
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Count (*) as Count\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join InternationalLicenses on InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID\r\n  " +
                $"join People on People.PersonID = Applications.ApplicantPersonID\r\n";
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

        private static string GetFilterConditionText(string filterkeyWord)
        {
            string filterCondition = $"\"{CurrentFilter}\" like '%{filterkeyWord}%'";
            return filterCondition.Length > 0 ? "\nwhere " + filterCondition : "";
        }

        public static string CurrentFilter { get; set; }

        public static int GetFilteredInternationalLicenceCount(string filterkeyWord)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select count(*) from (\n" +
                "SELECT  InternationalLicenses.InternationalLicenseID AS 'Int. licence ID'\r\n  " +
                ",Licenses.[ApplicationID] as 'Application ID'\r\n\t\t" +
                ",[IssuedUsingLocalLicenseID] as 'L.Licence ID'\r\n\t\t" +
                ",InternationalLicenses.IssueDate as 'Issue Date'\r\n\t\t" +
                ",InternationalLicenses.ExpirationDate as 'Expiration Date'\r\n\t\t" +
                ",InternationalLicenses.IsActive as 'Is Active'\r\n\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join InternationalLicenses on InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID\r\n  " +
                "join People on People.PersonID = Applications.ApplicantPersonID\r\n" +
                "\r) R1" + GetFilterConditionText(filterkeyWord); ;
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

        public static DataTable GetFilteredInternationalLicences(string filterkeyWord)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select * from (\n" +
                "SELECT  InternationalLicenses.InternationalLicenseID AS 'Int. licence ID'\r\n  " +
                ",Licenses.[ApplicationID] as 'Application ID'\r\n\t\t" +
                ",[IssuedUsingLocalLicenseID] as 'L.Licence ID'\r\n\t\t" +
                ",InternationalLicenses.IssueDate as 'Issue Date'\r\n\t\t" +
                ",InternationalLicenses.ExpirationDate as 'Expiration Date'\r\n\t\t" +
                ",InternationalLicenses.IsActive as 'Is Active'\r\n\r\n  " +
                "FROM [dvld].[dbo].[Licenses]\r\n  " +
                "join LicenseClasses on LicenseClasses.LicenseClassID = Licenses.LicenseClass\r\n  " +
                "join Applications on Applications.ApplicationID = Licenses.ApplicationID\r\n  " +
                "join InternationalLicenses on InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID\r\n  " +
                "join People on People.PersonID = Applications.ApplicantPersonID\r\n" +
                "\r) R1" + GetFilterConditionText(filterkeyWord) + SortingCondition;
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

        public static bool DoesInternationalLicenceExistWithLocalLicenceId(int licenceId)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "select found = 1\r\n" +
                "from InternationalLicenses \r\n" +
                "join Licenses on Licenses.LicenseID = InternationalLicenses.IssuedUsingLocalLicenseID " +
                "and Licenses.IsActive = 1 \r\n" +
                "where IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", licenceId);
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

        public static int GetLicenceClassId(int licenceId)
        {
            int licenceClassId = 0;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = $"Select [LicenseClass] From [Licenses] where [LicenseID] = {licenceId}";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    licenceClassId = (int)(dt.Rows[0][0] ?? null);
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

            return licenceClassId;
        }

        public static bool GetInternationalLicenceDataById(int id, ref int localLicenceId, ref string nationalNo, 
            ref string gender, ref DateTime issueDate, ref int applicationId, ref bool isActive,
            ref DateTime dateOfBirth, ref int driverId, ref DateTime expirationDate, ref string imagePath, ref string fullName)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [InternationalLicenseID]\r\n      " +
                ",InternationalLicenses.[ApplicationID]\r\n      " +
                ",[DriverID]\r\n      " +
                ",[IssuedUsingLocalLicenseID]\r\n      " +
                ",[IssueDate]\r\n      " +
                ",[ExpirationDate]\r\n      " +
                ",[IsActive]\r\n      " +
                ",CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName) AS 'Full Name'\r\n\t  " +
                ",People.NationalNo\r\n\t  " +
                ",CASE\r\n\t  " +
                "WHEN People.Gender = 0 THEN 'Female'\r\n\t  " +
                "ELSE 'Male'\t\r\n\t  " +
                "END AS Gender\r\n\t  " +
                ",People.ImagePath\r\n\t  " +
                ",People.DateOfBirth\r\n  " +
                "FROM [dvld].[dbo].[InternationalLicenses]\r\n  " +
                "join Applications on Applications.ApplicationID = InternationalLicenses.ApplicationID\r\n  " +
                "join People on Applications.ApplicantPersonID = People.PersonID\r\n  " +
                "where InternationalLicenseID = @InternationalLicenseID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@InternationalLicenseID", id);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    driverId = (int)reader["DriverID"];
                    localLicenceId = (int)reader["IssuedUsingLocalLicenseID"];
                    applicationId = (int)reader["ApplicationID"];
                    issueDate = (DateTime)reader["IssueDate"];
                    expirationDate = (DateTime)reader["ExpirationDate"];
                    isActive = (bool)reader["IsActive"];
                    gender = (string)reader["Gender"];
                    dateOfBirth = (DateTime)reader["DateOfBirth"];
                    nationalNo = (string)reader["NationalNo"];
                    fullName = (string)reader["Full Name"];

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

        public static bool DeactivateLicence(int id)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  Licenses  
                            set IsActive = 0    
                                where LicenseID = @LicenseID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LicenseID", id);

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
