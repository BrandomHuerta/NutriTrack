using NUTRITRACK.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NUTRITRACK
{
    public partial class TestDb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.CatalogoAlimentos", cn))
            {
                int n = (int)cmd.ExecuteScalar();
                lbl.Text = "Conexión OK. FoodCatalog rows: " + n;
            }
        }
    }
}