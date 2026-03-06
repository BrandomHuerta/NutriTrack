using NUTRITRACK.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Script.Serialization;

namespace NUTRITRACK.Pages
{
    public partial class Dashboard : System.Web.UI.Page
    {
        // ====== Totales del día ======
        public int PD_KcalTotal { get; private set; } = 0;
        public decimal PD_ProteinTotal { get; private set; } = 0m;
        public decimal PD_CarbsTotal { get; private set; } = 0m;
        public decimal PD_FatTotal { get; private set; } = 0m;

        // ====== % barras ======
        public int PD_KcalPct { get; private set; } = 0;
        public int PD_ProteinPct { get; private set; } = 0;
        public int PD_CarbsPct { get; private set; } = 0;
        public int PD_FatPct { get; private set; } = 0;

        // ====== Metas ======
        public int GoalKcal { get; private set; } = 2000;
        public decimal GoalProtein { get; private set; } = 150m;
        public decimal GoalCarbs { get; private set; } = 200m;
        public decimal GoalFat { get; private set; } = 70m;

        // ====== Pie ======
        public string PD_PieLabelsJson { get; private set; } = "[]";
        public string PD_PieValuesJson { get; private set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect(ResolveUrl("~/Pages/Auth/Login.aspx"));
                return;
            }

            if (!IsPostBack)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DateTime today = DateTime.Today;

                LoadGoals(userId);              // <-- YA EN INGLÉS
                LoadTodayTotals(userId, today); // <-- FoodLogs
                LoadRecentFoods(userId, today); // <-- lista en "Resumen"
                BuildPieJson();                 // <-- colores consistentes con barras
            }
        }

        private void LoadGoals(int userId)
        {
            // Defaults si no hay metas o si la tabla no existe bien.
            GoalKcal = 2000;
            GoalProtein = 150m;
            GoalCarbs = 200m;
            GoalFat = 70m;

            try
            {
                using (var cn = Db.Open())
                {
                    // 1) Detectar el nombre real de la columna FK del usuario
                    string userCol = GetFirstExistingColumn(cn, "UserGoals", "UserId", "UsuarioId", "IdUsuario");
                    if (string.IsNullOrEmpty(userCol))
                        return; // no se puede filtrar por usuario (no hay col)

                    // 2) Traer fila completa sin mencionar columnas que tal vez no existan
                    string sql = $@"
SELECT TOP 1 *
FROM dbo.UserGoals
WHERE {userCol} = @UserId
ORDER BY 1 DESC;";

                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return;

                            // 3) Leer metas usando el primer nombre que exista (inglés o español)
                            GoalKcal = SafeInt(ReadAny(r,
                                "DailyCalories", "CaloriesDaily", "CaloriasDiarias", "CaloriasDiariasGoal", "GoalCalories"), GoalKcal);

                            GoalProtein = SafeDec(ReadAny(r,
                                "DailyProtein", "ProteinDaily", "ProteinaDiaria", "ProteinaDiariaGoal", "GoalProtein"), GoalProtein);

                            GoalCarbs = SafeDec(ReadAny(r,
                                "DailyCarbs", "CarbsDaily", "CarbohidratosDiarios", "CarbohidratosDiariosGoal", "GoalCarbs"), GoalCarbs);

                            GoalFat = SafeDec(ReadAny(r,
                                "DailyFat", "FatDaily", "GrasasDiarias", "GrasasDiariasGoal", "GoalFat"), GoalFat);
                        }
                    }
                }
            }
            catch
            {
                // Si algo falla, te quedas con defaults y NO truena el dashboard.
                // (Así ya no verás la pantalla amarilla)
            }
        }

        private void LoadTodayTotals(int userId, DateTime day)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT
  ISNULL(SUM(Calories),0) AS Kcal,
  ISNULL(SUM(Protein),0)  AS Prot,
  ISNULL(SUM(Carbs),0)    AS Carbs,
  ISNULL(SUM(Fat),0)      AS Fat
FROM dbo.FoodLogs
WHERE UserId = @UserId
  AND LogDate = @Day;", cn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Day", SqlDbType.Date).Value = day.Date;

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        PD_KcalTotal = SafeInt(r["Kcal"], 0);
                        PD_ProteinTotal = SafeDec(r["Prot"], 0m);
                        PD_CarbsTotal = SafeDec(r["Carbs"], 0m);
                        PD_FatTotal = SafeDec(r["Fat"], 0m);
                    }
                }
            }

            PD_KcalPct = CalcPct(PD_KcalTotal, GoalKcal);

            PD_ProteinPct = CalcPct((double)PD_ProteinTotal, (double)GoalProtein);
            PD_CarbsPct = CalcPct((double)PD_CarbsTotal, (double)GoalCarbs);
            PD_FatPct = CalcPct((double)PD_FatTotal, (double)GoalFat);
        }

        private void LoadRecentFoods(int userId, DateTime day)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT TOP 20
  FoodName AS FoodName,
  Grams    AS Grams,
  Protein  AS Protein_g,
  Carbs    AS Carbs_g,
  Fat      AS Fat_g,
  Calories AS Kcal,
  CONVERT(varchar(5), CreatedAt, 108) AS [Time]
FROM dbo.FoodLogs
WHERE UserId = @UserId
  AND LogDate = @Day
ORDER BY CreatedAt DESC;", cn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Day", SqlDbType.Date).Value = day.Date;

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    repRecentFoods.DataSource = dt;
                    repRecentFoods.DataBind();

                    pnlEmptyRecent.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        private void BuildPieJson()
        {
            // Colores deben coincidir con las barras:
            // Protein: verde (#198754)
            // Carbs: amarillo (#ffc107)
            // Fat: naranja (#fd7e14)
            // Calories: azul (#0d6efd)
            var labels = new[] { "Proteína", "Carbs", "Grasa", "Calorías" };

            // (si quieres que el pastel sea "kcal por macro", dímelo y lo cambiamos)
            var values = new object[]
            {
                (double)PD_ProteinTotal,
                (double)PD_CarbsTotal,
                (double)PD_FatTotal,
                (double)PD_KcalTotal
            };

            var js = new JavaScriptSerializer();
            PD_PieLabelsJson = js.Serialize(labels);
            PD_PieValuesJson = js.Serialize(values);
        }

        // ===== Helpers =====
        public string Format0(object n)
        {
            double v = 0;
            if (n != null) double.TryParse(Convert.ToString(n), out v);
            return Math.Round(v, 0).ToString("0", CultureInfo.InvariantCulture);
        }

        public string Format1(object n)
        {
            double v = 0;
            if (n != null) double.TryParse(Convert.ToString(n), out v);
            return Math.Round(v, 1).ToString("0.0", CultureInfo.InvariantCulture);
        }

        private int CalcPct(double total, double goal)
        {
            if (goal <= 0) return 0;
            return (int)Math.Min(100, Math.Round((total / goal) * 100.0, 0));
        }

        private int SafeInt(object o, int def)
        {
            if (o == null || o == DBNull.Value) return def;
            int v;
            return int.TryParse(Convert.ToString(o), out v) ? v : def;
        }

        private decimal SafeDec(object o, decimal def)
        {
            if (o == null || o == DBNull.Value) return def;
            decimal v;
            return decimal.TryParse(Convert.ToString(o), NumberStyles.Any, CultureInfo.InvariantCulture, out v) ? v : def;
        }

        private string GetFirstExistingColumn(SqlConnection cn, string tableName, params string[] candidates)
        {
            foreach (var c in candidates)
            {
                if (ColumnExists(cn, tableName, c)) return c;
            }
            return null;
        }

        private bool ColumnExists(SqlConnection cn, string tableName, string columnName)
        {
            using (var cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = @T
  AND COLUMN_NAME = @C;", cn))
            {
                cmd.Parameters.Add("@T", SqlDbType.NVarChar, 128).Value = tableName;
                cmd.Parameters.Add("@C", SqlDbType.NVarChar, 128).Value = columnName;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private object ReadAny(SqlDataReader r, params string[] names)
        {
            foreach (var n in names)
            {
                int ord = GetOrdinalSafe(r, n);
                if (ord >= 0 && !r.IsDBNull(ord)) return r.GetValue(ord);
            }
            return null;
        }

        private int GetOrdinalSafe(SqlDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }
    }
}