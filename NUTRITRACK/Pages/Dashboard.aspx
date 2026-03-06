<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master"
  AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs"
  Inherits="NUTRITRACK.Pages.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <div class="d-flex align-items-center justify-content-between mb-2">
    <div>
      <h2 class="m-0">
        <i class="fa-solid fa-gauge-high me-2"></i>Panel Principal
      </h2>
      <div class="text-muted">Resumen de tu día en NutriTrack</div>
    </div>
  </div>

  <div class="row g-3">

    <!-- Acciones rápidas -->
    <div class="col-md-4">
      <div class="card shadow-sm">
        <div class="card-body">
          <div class="fw-bold mb-2">Acciones rápidas</div>

          <div class="d-grid gap-2">
            <a class="btn btn-outline-dark"
               href="<%= ResolveUrl("~/Pages/Food/FoodSearch.aspx") %>">
              <i class="fa-solid fa-utensils me-2"></i>Registrar comida del día
            </a>

            <a class="btn btn-outline-dark"
               href="<%= ResolveUrl("~/Pages/Stats/WeeklyProgress.aspx") %>">
              <i class="fa-solid fa-chart-column me-2"></i>Progreso semanal
            </a>
          </div>
        </div>
      </div>
    </div>

    <!-- Resumen recientes -->
    <div class="col-md-8">
      <div class="card shadow-sm">
        <div class="card-body">
          <div class="fw-bold d-flex justify-content-between align-items-center">
            <span>Resumen</span>
            <span class="text-muted small">Recientes</span>
          </div>

          <asp:Panel ID="pnlEmptyRecent" runat="server" CssClass="text-muted mt-2" Visible="false">
            Aún no has registrado alimentos hoy.
          </asp:Panel>

          <asp:Repeater ID="repRecentFoods" runat="server">
            <HeaderTemplate>
              <div class="list-group mt-2">
            </HeaderTemplate>

            <ItemTemplate>
              <div class="list-group-item d-flex justify-content-between align-items-start">
                <div class="me-3">
                  <div class="fw-semibold"><%# Eval("FoodName") %></div>

                  <div class="text-muted small">
                    <%# Eval("Grams") %> g
                    &nbsp;•&nbsp; Prot: <%# Eval("Protein_g") %> g
                    &nbsp;•&nbsp; Carb: <%# Eval("Carbs_g") %> g
                    &nbsp;•&nbsp; Grasa: <%# Eval("Fat_g") %> g
                  </div>
                </div>

                <div class="text-end">
                  <div class="fw-bold"><%# Eval("Kcal") %> kcal</div>
                  <div class="text-muted small"><%# Eval("Time") %></div>
                </div>
              </div>
            </ItemTemplate>

            <FooterTemplate>
              </div>
            </FooterTemplate>
          </asp:Repeater>

        </div>
      </div>
    </div>

    <!-- Progreso diario -->
    <div class="col-md-6">
      <div class="card shadow-sm">
        <div class="card-body">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <div class="fw-bold">
              <i class="fa-solid fa-chart-line me-2"></i>Progreso diario
            </div>
            <div class="text-muted small">Hoy</div>
          </div>

          <!-- Calorías (AZUL) -->
          <div class="d-flex justify-content-between">
            <div>Calorías</div>
            <div class="text-muted">
              <%= Format0(PD_KcalTotal) %> / <%= Format0(GoalKcal) %> kcal
            </div>
          </div>
          <div class="progress mb-3" style="height:10px;">
            <div class="progress-bar bg-primary" role="progressbar"
                 style='width: <%= PD_KcalPct %>%;'
                 aria-valuenow="<%= PD_KcalPct %>" aria-valuemin="0" aria-valuemax="100"></div>
          </div>

          <!-- Proteína (VERDE) -->
          <div class="d-flex justify-content-between">
            <div>Proteína</div>
            <div class="text-muted">
              <%= Format1(PD_ProteinTotal) %> / <%= Format1(GoalProtein) %> g
            </div>
          </div>
          <div class="progress mb-3" style="height:10px;">
            <div class="progress-bar bg-success" role="progressbar"
                 style='width: <%= PD_ProteinPct %>%;'
                 aria-valuenow="<%= PD_ProteinPct %>" aria-valuemin="0" aria-valuemax="100"></div>
          </div>

          <!-- Carbohidratos (AMARILLO) -->
          <div class="d-flex justify-content-between">
            <div>Carbohidratos</div>
            <div class="text-muted">
              <%= Format1(PD_CarbsTotal) %> / <%= Format1(GoalCarbs) %> g
            </div>
          </div>
          <div class="progress mb-2" style="height:10px;">
            <div class="progress-bar bg-warning" role="progressbar"
                 style='width: <%= PD_CarbsPct %>%;'
                 aria-valuenow="<%= PD_CarbsPct %>" aria-valuemin="0" aria-valuemax="100"></div>
          </div>

          <!-- Grasa (NARANJA) -->
          <div class="d-flex justify-content-between mt-2">
            <div>Grasa</div>
            <div class="text-muted">
              <%= Format1(PD_FatTotal) %> / <%= Format1(GoalFat) %> g
            </div>
          </div>
          <div class="progress mb-2" style="height:10px;">
            <div class="progress-bar" role="progressbar"
                 style='width: <%= PD_FatPct %>%; background-color:#fd7e14;'
                 aria-valuenow="<%= PD_FatPct %>" aria-valuemin="0" aria-valuemax="100"></div>
          </div>

          <div class="text-muted small mt-3">
            <i class="fa-solid fa-circle-info me-1"></i>
            Las barras representan tu avance contra la meta del día.
          </div>
        </div>
      </div>
    </div>

    <!-- Pastel -->
    <div class="col-md-6">
      <div class="card shadow-sm">
        <div class="card-body">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <div class="fw-bold">
              <i class="fa-solid fa-chart-pie me-2"></i>Distribución (Pastel)
            </div>
            <div class="text-muted small">kcal por macro + total</div>
          </div>

          <canvas id="macroPie" height="220"></canvas>

          <div class="mt-3">
            <div><b>Carbs:</b> <span id="lblCarbs"><%= Format1(PD_CarbsTotal) %></span> g</div>
            <div><b>Proteína:</b> <span id="lblProt"><%= Format1(PD_ProteinTotal) %></span> g</div>
            <div><b>Grasa:</b> <span id="lblFat"><%= Format1(PD_FatTotal) %></span> g</div>
            <div><b>Calorías:</b> <span id="lblKcal"><%= Format0(PD_KcalTotal) %></span> kcal</div>
          </div>
        </div>
      </div>
    </div>

  </div>

  <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

  <!-- Pastel: colores iguales a barras -->
  <script type="text/javascript">
    (function () {
      if (typeof Chart === "undefined") return;

      var labels = <%= PD_PieLabelsJson %>;
      var values = <%= PD_PieValuesJson %>;

          var canvas = document.getElementById("macroPie");
          if (!canvas) return;

          if (!values || values.length !== 4) return;

          var total = 0;
          for (var i = 0; i < values.length; i++) total += (values[i] || 0);
          if (total <= 0) return;

          // Proteína (verde), Carbs (amarillo), Grasa (naranja), Calorías (azul)
          var colors = ["#198754", "#ffc107", "#fd7e14", "#0d6efd"];

          new Chart(canvas, {
              type: "doughnut",
              data: {
                  labels: labels,
                  datasets: [{
                      data: values,
                      backgroundColor: colors,
                      borderWidth: 0
                  }]
              },
              options: {
                  responsive: true,
                  plugins: { legend: { position: "bottom" } },
                  cutout: "55%"
              }
          });
      })();
  </script>

</asp:Content>