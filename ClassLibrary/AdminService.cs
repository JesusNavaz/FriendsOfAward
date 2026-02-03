using System.Data;

namespace ClassLibrary
{
    public class AdminService
    {
        public static bool IsValidAdmin(string email, string password)
        {
            DbWrapperMySqlV2 db = DbWrapperMySqlV2.Wrapper;

            string sql = $"SELECT * FROM foa_admins WHERE email = '{email}' AND password = '{password}'";

            DataTable result = db.RunQuery(sql);

            return result.Rows.Count > 0;
        }
    }
}