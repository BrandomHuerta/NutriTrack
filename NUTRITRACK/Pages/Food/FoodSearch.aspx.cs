using NUTRITRACK.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace NUTRITRACK.Pages.Food
{
    public partial class FoodSearch : System.Web.UI.Page
    {
        public class FoodSuggestItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect(ResolveUrl("~/Pages/Auth/Login.aspx"));
                return;
            }

            if (!IsPostBack)
            {
                lblMsg.Text = "";
                repResults.DataSource = null;
                repResults.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string q = (txtSearch.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(q))
            {
                lblMsg.CssClass = "text-danger d-block mb-2";
                lblMsg.Text = "Escribe el nombre de un alimento.";
                repResults.DataSource = null;
                repResults.DataBind();
                return;
            }

            int foodId;
            if (!int.TryParse(hfFoodId.Value, out foodId))
            {
                foodId = FindFoodIdExact(q);
                if (foodId <= 0)
                {
                    lblMsg.CssClass = "text-danger d-block mb-2";
                    lblMsg.Text = "No existe un alimento con ese nombre. Intenta con otro.";
                    repResults.DataSource = null;
                    repResults.DataBind();
                    return;
                }
            }

            BindFoodById(foodId);

            lblMsg.CssClass = "text-muted d-block mb-2";
            lblMsg.Text = "Resultado:";
        }

        private int FindFoodIdExact(string name)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT TOP 1 Id
FROM dbo.FoodCatalog
WHERE IsActive = 1
  AND FoodName = @name;", cn))
            {
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = name;

                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return 0;
                return Convert.ToInt32(o);
            }
        }

        private void BindFoodById(int foodId)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT
  Id,
  FoodName AS [Name],
  CaloriesPer100g AS CaloriesKcal_100g,
  ProteinPer100g  AS Protein_100g,
  CarbsPer100g    AS Carbs_100g,
  FatPer100g      AS Fat_100g
FROM dbo.FoodCatalog
WHERE IsActive = 1
  AND Id = @id;", cn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = foodId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    repResults.DataSource = dt;
                    repResults.DataBind();
                }
            }
        }

        protected void repResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "add") return;

            if (Session["UserId"] == null)
            {
                Response.Redirect(ResolveUrl("~/Pages/Auth/Login.aspx"));
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            int foodId = Convert.ToInt32(e.CommandArgument);

            decimal grams = 100m;
            TextBox txt = e.Item.FindControl("txtGrams") as TextBox;
            if (txt != null)
            {
                decimal parsed;
                // Soporta "100", "100.5", "100,5"
                if (decimal.TryParse((txt.Text ?? "100").Trim(),
                        NumberStyles.Number, CultureInfo.InvariantCulture, out parsed) ||
                    decimal.TryParse((txt.Text ?? "100").Trim(),
                        NumberStyles.Number, new CultureInfo("es-MX"), out parsed))
                {
                    if (parsed > 0) grams = parsed;
                }
            }

            AddFoodLog(userId, foodId, grams);

            // ✅ Regresa al dashboard correcto (evita 404)
            Response.Redirect(ResolveUrl("~/Pages/Dashboard.aspx"));
        }

        private void AddFoodLog(int userId, int foodId, decimal grams)
        {
            using (var cn = Db.Open())
            {
                // 1) Traer info del alimento (por 100g)
                string foodName;
                int cals100;
                decimal prot100, carbs100, fat100;

                using (var cmdFood = new SqlCommand(@"
SELECT TOP 1
  FoodName,
  CaloriesPer100g,
  ProteinPer100g,
  ISNULL(CarbsPer100g,0) AS CarbsPer100g,
  ISNULL(FatPer100g,0)   AS FatPer100g
FROM dbo.FoodCatalog
WHERE IsActive = 1 AND Id = @FoodId;", cn))
                {
                    cmdFood.Parameters.Add("@FoodId", SqlDbType.Int).Value = foodId;

                    using (var r = cmdFood.ExecuteReader())
                    {
                        if (!r.Read())
                            throw new Exception("El alimento no existe o está inactivo.");

                        foodName = Convert.ToString(r["FoodName"] ?? "");
                        cals100 = Convert.ToInt32(r["CaloriesPer100g"]);
                        prot100 = Convert.ToDecimal(r["ProteinPer100g"]);
                        carbs100 = Convert.ToDecimal(r["CarbsPer100g"]);
                        fat100 = Convert.ToDecimal(r["FatPer100g"]);
                    }
                }

                // 2) Calcular valores por gramos
                decimal factor = grams / 100m;

                int calories = (int)Math.Round(cals100 * factor, 0);
                decimal protein = Math.Round(prot100 * factor, 2);
                decimal carbs = Math.Round(carbs100 * factor, 2);
                decimal fat = Math.Round(fat100 * factor, 2);

                // 3) Insert a dbo.FoodLogs (la tabla buena)
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.FoodLogs
(
  UserId, LogDate,
  FoodId, FoodName, Grams,
  Calories, Protein, Carbs, Fat,
  CreatedAt
)
VALUES
(
  @UserId, CAST(GETDATE() AS date),
  @FoodId, @FoodName, @Grams,
  @Calories, @Protein, @Carbs, @Fat,
  SYSUTCDATETIME()
);", cn))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@FoodId", SqlDbType.Int).Value = foodId;
                    cmd.Parameters.Add("@FoodName", SqlDbType.NVarChar, 200).Value = foodName;

                    var pGr = cmd.Parameters.Add("@Grams", SqlDbType.Decimal);
                    pGr.Precision = 10; pGr.Scale = 2; pGr.Value = grams;

                    cmd.Parameters.Add("@Calories", SqlDbType.Int).Value = calories;

                    var pP = cmd.Parameters.Add("@Protein", SqlDbType.Decimal);
                    pP.Precision = 10; pP.Scale = 2; pP.Value = protein;

                    var pC = cmd.Parameters.Add("@Carbs", SqlDbType.Decimal);
                    pC.Precision = 10; pC.Scale = 2; pC.Value = carbs;

                    var pF = cmd.Parameters.Add("@Fat", SqlDbType.Decimal);
                    pF.Precision = 10; pF.Scale = 2; pF.Value = fat;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        [WebMethod]
        public static List<FoodSuggestItem> SearchFoods(string q)
        {
            var list = new List<FoodSuggestItem>();

            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return list;

            q = q.Trim();

            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT TOP 12 Id, FoodName
FROM dbo.FoodCatalog
WHERE IsActive = 1
  AND FoodName LIKE @q + '%'
ORDER BY FoodName;", cn))
            {
                cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = q;

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new FoodSuggestItem
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Name = Convert.ToString(r["FoodName"])
                        });
                    }
                }
            }

            return list;
        }
    }
}