using NUTRITRACK.DAL;
using System;

namespace NUTRITRACK.Pages.Auth
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly UserRepository _userRepository = new UserRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            lblMsg.CssClass = "d-none";

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                lblMsg.Text = "Completa email y contraseña.";
                lblMsg.CssClass = "d-block";
                return;
            }

            // Si tú guardaste el password como hash, aquí debes aplicar el mismo hash antes de validar.
            // Por ahora lo dejo directo porque así estás trabajando en tu repositorio actual.
            string passwordHash = password;

            int? userId = _userRepository.ValidateLogin(email, passwordHash);

            if (!userId.HasValue)
            {
                lblMsg.Text = "Correo o contraseña incorrectos.";
                lblMsg.CssClass = "d-block";
                return;
            }

            Session["UserId"] = userId.Value;
            Session["UserEmail"] = email;

            Response.Redirect(ResolveUrl("~/Pages/Splash/Splash.aspx"));
        }
    }
}