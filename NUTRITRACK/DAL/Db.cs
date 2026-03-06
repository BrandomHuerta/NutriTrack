using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NUTRITRACK.DAL
{
    public static class Db
    {
        public static SqlConnection Open()
        {
            string cs = ConfigurationManager.ConnectionStrings["NutriTrackDB"].ConnectionString;

            var cn = new SqlConnection(cs);
            cn.Open();
            return cn;
        }
    }
}
