using System.Data.SqlClient;
using System.Data;
using System;

namespace DLMS_DataAccess
{
    public class LicenceClassData
    {
        public static DataTable GetAllLicenceClassesForDropDown()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT [LicenseClassID]\r\n      ,[ClassName]\r\n  FROM [dvld].[dbo].[LicenseClasses]";
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
    }
}
