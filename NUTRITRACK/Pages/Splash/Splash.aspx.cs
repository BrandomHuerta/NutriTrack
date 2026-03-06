using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NUTRITRACK.Pages.Splash
{
    public partial class Splash : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // WebForms no soporta async Page_Load directo sin hacks.
            // Lo resolvemos con GetAwaiter().GetResult() en algo corto.
            if (!IsPostBack)
            {
                LoadTipSafe();
                RedirectNext();
            }
        }

        private void LoadTipSafe()
        {
            try
            {
                //var svc = new OpenFoodFactsService();
                //var tip = svc.GetQuickTipAsync().GetAwaiter().GetResult();
                //lblTip.Text = tip;
            }
            catch
            {
                lblTip.Text = "Tip: registra tu comida por gramos para mejor precisión.";
            }
        }

        private void RedirectNext()
        {
            // Espera visual corta (sin bloquear demasiado)
            // Nota: Response.AddHeader para refrescar en 2 segundos.
            string next = (Session["UserId"] != null)
                ? ResolveUrl("~/Pages/Dashboard.aspx")
                : ResolveUrl("~/Pages/Auth/Login.aspx");

            Response.AddHeader("REFRESH", "2;URL=" + next);
        }
    }
}