<%@ Page Title="Crear cuenta" Language="C#" MasterPageFile="~/Site1.Master"
  AutoEventWireup="true" CodeBehind="Register.aspx.cs"
  Inherits="NUTRITRACK.Pages.Auth.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <div class="d-flex justify-content-center">
    <div class="card shadow-sm" style="max-width:420px; width:100%;">
      <div class="card-body p-4">

        <h3 class="mb-2">
          <i class="fa-solid fa-user-plus me-2"></i>Crear cuenta
        </h3>

        <div class="alert alert-info py-2">
          <i class="fa-solid fa-circle-info me-2"></i>
          Agregue sus datos personales para crear su cuenta en <b>NutriTrack</b>.
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="d-block mb-2"></asp:Label>

        <!-- Nombre -->
        <div class="mb-3">
          <label class="form-label">Nombre</label>
          <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
        </div>

        <!-- Email -->
        <div class="mb-3">
          <label class="form-label">Email</label>
          <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
        </div>

        <!-- Contraseña -->
        <div class="mb-3">
          <label class="form-label">Contraseña</label>
          <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
        </div>

        <!-- ✅ Edad -->
        <div class="mb-3">
          <label class="form-label">Edad</label>
          <asp:TextBox ID="txtEdad" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 25" />
        </div>

        <!-- ✅ Altura -->
        <div class="mb-3">
          <label class="form-label">Altura (cm)</label>
          <asp:TextBox ID="txtAlturaCm" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 175" />
        </div>

        <!-- ✅ Peso -->
        <div class="mb-3">
          <label class="form-label">Peso (kg)</label>
          <asp:TextBox ID="txtPesoKg" runat="server" CssClass="form-control" placeholder="Ej: 80.5" />
        </div>

        <!-- ✅ Objetivo -->
        <div class="mb-3">
          <label class="form-label">Objetivo</label>
          <asp:DropDownList ID="ddlObjetivo" runat="server" CssClass="form-select">
            <asp:ListItem Text="Selecciona..." Value="" />
            <asp:ListItem Text="Bajar de peso" Value="Bajar de peso" />
            <asp:ListItem Text="Subir masa muscular" Value="Subir masa muscular" />
            <asp:ListItem Text="Mantener" Value="Mantener" />
            <asp:ListItem Text="Mejorar rendimiento" Value="Mejorar rendimiento" />
          </asp:DropDownList>
        </div>

        <asp:Button ID="btnRegister" runat="server"
          CssClass="btn btn-dark w-100"
          Text="Registrar"
          OnClick="btnRegister_Click" />

        <div class="text-center mt-3">
          <a href="<%= ResolveUrl("~/Pages/Auth/Login.aspx") %>">¿Ya tienes cuenta? Inicia sesión</a>
        </div>

      </div>
    </div>
  </div>

</asp:Content>