using NUTRITRACK.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace NUTRITRACK.Pages.Auth
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMsg.Text = "";
                lblMsg.CssClass = "d-block mb-2";
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string nombre = (txtName.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string pass = (txtPassword.Text ?? "").Trim();
            string objetivo = (ddlObjetivo.SelectedValue ?? "").Trim();

            int edad;
            int alturaCm;
            decimal pesoKg;

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
            {
                ShowError("Completa Nombre, Email y Contraseña.");
                return;
            }

            if (!int.TryParse((txtEdad.Text ?? "").Trim(), out edad) || edad < 1 || edad > 120)
            {
                ShowError("Edad inválida (1 a 120).");
                return;
            }

            if (!int.TryParse((txtAlturaCm.Text ?? "").Trim(), out alturaCm) || alturaCm < 50 || alturaCm > 260)
            {
                ShowError("Altura inválida (50 a 260 cm).");
                return;
            }

            string pesoTxt = (txtPesoKg.Text ?? "").Trim();
            if (!decimal.TryParse(pesoTxt, NumberStyles.Any, CultureInfo.InvariantCulture, out pesoKg) &&
                !decimal.TryParse(pesoTxt, NumberStyles.Any, new CultureInfo("es-MX"), out pesoKg))
            {
                ShowError("Peso inválido. Ej: 80.5");
                return;
            }
            if (pesoKg < 10 || pesoKg > 400)
            {
                ShowError("Peso inválido (10 a 400 kg).");
                return;
            }

            if (string.IsNullOrWhiteSpace(objetivo))
            {
                ShowError("Selecciona un objetivo.");
                return;
            }

            try
            {
                using (var cn = Db.Open())
                using (var tx = cn.BeginTransaction())
                {
                    // 1) email duplicado
                    using (var check = new SqlCommand("SELECT COUNT(1) FROM dbo.Users WHERE Email=@Email;", cn, tx))
                    {
                        check.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                        int exists = Convert.ToInt32(check.ExecuteScalar());
                        if (exists > 0)
                        {
                            tx.Rollback();
                            ShowError("Ese email ya está registrado.");
                            return;
                        }
                    }

                    // 2) insertar usuario + obtener Id
                    int newUserId = 0;

                    using (var cmd = new SqlCommand(@"
INSERT INTO dbo.Users
(
  Email,
  PasswordHash,
  FullName,
  Age,
  WeightKg,
  HeightCm,
  Goal,
  IsActive,
  CreatedAt
)
OUTPUT INSERTED.Id
VALUES
(
  @Email,
  @Pass,
  @Nombre,
  @Edad,
  @PesoKg,
  @AlturaCm,
  @Objetivo,
  1,
  SYSUTCDATETIME()
);", cn, tx))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                        cmd.Parameters.Add("@Pass", SqlDbType.NVarChar, 500).Value = pass;
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value = nombre;

                        cmd.Parameters.Add("@Edad", SqlDbType.Int).Value = edad;

                        var pPeso = cmd.Parameters.Add("@PesoKg", SqlDbType.Decimal);
                        pPeso.Precision = 6;
                        pPeso.Scale = 2;
                        pPeso.Value = pesoKg;

                        cmd.Parameters.Add("@AlturaCm", SqlDbType.Decimal).Value = Convert.ToDecimal(alturaCm); // HeightCm es DECIMAL
                        cmd.Parameters.Add("@Objetivo", SqlDbType.NVarChar, 100).Value = objetivo;

                        newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 3) crear metas (defaults o calculo simple)
                    // Si luego haces cálculo real, aquí lo ajustamos.
                    int kcal = 2000;
                    decimal prot = 140m;
                    decimal carbs = 250m;
                    decimal grasa = 70m;

                    using (var g = new SqlCommand(@"
INSERT INTO dbo.UserGoals
(
  UserId,
  KcalObjetivo,
  ProteinaObjetivo,
  CarbsObjetivo,
  GrasaObjetivo,
  CreatedAt
)
VALUES
(
  @UserId,
  @Kcal,
  @Prot,
  @Carbs,
  @Grasa,
  SYSUTCDATETIME()
);", cn, tx))
                    {
                        g.Parameters.Add("@UserId", SqlDbType.Int).Value = newUserId;
                        g.Parameters.Add("@Kcal", SqlDbType.Int).Value = kcal;

                        var p1 = g.Parameters.Add("@Prot", SqlDbType.Decimal);
                        p1.Precision = 10; p1.Scale = 1; p1.Value = prot;

                        var p2 = g.Parameters.Add("@Carbs", SqlDbType.Decimal);
                        p2.Precision = 10; p2.Scale = 1; p2.Value = carbs;

                        var p3 = g.Parameters.Add("@Grasa", SqlDbType.Decimal);
                        p3.Precision = 10; p3.Scale = 1; p3.Value = grasa;

                        g.ExecuteNonQuery();
                    }

                    tx.Commit();
                }

                // ✅ manda a login
                Response.Redirect(ResolveUrl("~/Pages/Auth/Login.aspx"));
            }
            catch (Exception ex)
            {
                ShowError("Error al registrar: " + ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            lblMsg.CssClass = "alert alert-danger";
            lblMsg.Text = msg;
        }
    }
}