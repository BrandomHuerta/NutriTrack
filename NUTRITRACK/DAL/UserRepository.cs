using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NUTRITRACK.DAL
{
    public class UserRepository
    {
        public int CreateUser(string email, string passwordHash, string fullName)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.Users(Email, PasswordHash, FullName)
VALUES(@Email,@PasswordHash,@FullName);
SELECT CAST(SCOPE_IDENTITY() AS INT);", cn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@FullName", fullName);

                return (int)cmd.ExecuteScalar();
            }
        }

        public int? ValidateLogin(string email, string passwordHash)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT Id
FROM dbo.Users
WHERE Email=@Email AND PasswordHash=@PasswordHash AND IsActive=1;", cn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                var obj = cmd.ExecuteScalar();
                return obj == null ? (int?)null : Convert.ToInt32(obj);
            }
        }
    }
}