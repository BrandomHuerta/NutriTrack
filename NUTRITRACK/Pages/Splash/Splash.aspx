<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Splash.aspx.cs" Inherits="NUTRITRACK.Pages.Splash.Splash" %>

<!DOCTYPE html>
<html>
<head runat="server">
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />

  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>
  <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet"/>

  <title>NutriTrack - Cargando</title>

  <style>
    body{
      background:#0b1220;
      color:#fff;
    }

    .cardx{
      background:rgba(255,255,255,.06);
      border:1px solid rgba(255,255,255,.12);
      border-radius:18px;
    }

    .muted{
      color:rgba(255,255,255,.75);
    }

    /* contenedor animación */
    .pushup-container{
      margin-top:25px;
      height:140px;
      position:relative;
      display:flex;
      justify-content:center;
      align-items:center;
    }

      .pushup{

    width:220px;
    position:absolute;
    left:50%;
    transform:translateX(-50%);

      }

    /* fase concentrica (arriba) */
    .pushup-concentrico{
      animation:pushupUp 1s infinite;
    }

    /* fase excentrica (abajo) */
    .pushup-excentrico{
      animation:pushupDown 1s infinite;
    }

    @keyframes pushupUp{
      0%{opacity:1;}
      50%{opacity:0;}
      100%{opacity:1;}
    }

    @keyframes pushupDown{
      0%{opacity:0;}
      50%{opacity:1;}
      100%{opacity:0;}
    }

    .floor{
      width:200px;
      height:6px;
      background:#4ade80;
      border-radius:999px;
      margin:6px auto 0;
      opacity:.9;
    }

    .spinner-border{
      width:3rem;
      height:3rem;
    }
  </style>
</head>

<body>
  <form id="form1" runat="server">
    <div class="d-flex align-items-center justify-content-center min-vh-100 p-3">
      <div class="cardx p-4 p-md-5 text-center" style="max-width:560px; width:100%;">

        <div class="display-6 fw-bold mb-2">
          <i class="fa-solid fa-apple-whole me-2"></i>NutriTrack
        </div>

        <div class="muted mb-4">Cargando recursos...</div>

        <div class="spinner-border" role="status" aria-hidden="true"></div>

        <!-- ANIMACIÓN DE LAGARTIJAS -->
        <div class="pushup-container">
          <!-- OJO: tu carpeta es Image, no Images -->
          <img src="<%=ResolveClientUrl("~/Image/pushup_exentrico.png")%>" class="pushup pushup-exentrico" alt="pushup down"/>
              <img src="<%=ResolveClientUrl("~/Image/pushup_concentrico.png")%>" class="pushup pushup-concentrico" alt="pushup up"/>
        </div>

        <div class="floor"></div>

        <asp:Label ID="lblTip" runat="server" CssClass="mt-3 d-block"></asp:Label>

        <div class="mt-4 muted small">
          <i class="fa-solid fa-shield-halved me-1"></i>
          Los datos se procesarán de manera local, espere unos segundos.
        </div>

      </div>
    </div>
  </form>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>