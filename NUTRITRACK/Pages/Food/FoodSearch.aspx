<%@ Page Title="Registrar comida del día" Language="C#" MasterPageFile="~/Site1.Master"
AutoEventWireup="true" CodeBehind="FoodSearch.aspx.cs" Inherits="NUTRITRACK.Pages.Food.FoodSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<h3 class="mb-3">
<i class="fa-solid fa-magnifying-glass me-2"></i>Registrar comida del día
</h3>

<asp:Label ID="lblMsg" runat="server" CssClass="d-block mb-2"></asp:Label>

<div class="position-relative mb-2">

<div class="input-group">

<asp:TextBox
ID="txtSearch"
runat="server"
CssClass="form-control"
placeholder="Ej: avena, yogurt, pechuga..."
autocomplete="off" />

<asp:Button
ID="btnSearch"
runat="server"
CssClass="btn btn-dark"
Text="Buscar"
OnClick="btnSearch_Click" />

</div>

<asp:HiddenField ID="hfFoodId" runat="server" />

<div id="foodSuggestBox"
class="list-group position-absolute w-100 shadow"
style="z-index:2000; display:none; max-height:280px; overflow:auto;">
</div>

</div>


<div class="mt-3">

<asp:Repeater ID="repResults" runat="server" OnItemCommand="repResults_ItemCommand">

<ItemTemplate>

<div class="card mb-2">

<div class="card-body">

<div class="fw-bold">
<%# Eval("Name") %>
</div>

<div class="text-muted small">

kcal/100g:
<%# Eval("CaloriesKcal_100g") %>

|

Prot:
<%# Eval("Protein_100g") %>g

|

Carb:
<%# Eval("Carbs_100g") %>g

|

Grasa:
<%# Eval("Fat_100g") %>g

</div>

<div class="d-flex gap-2 align-items-center mt-2 flex-wrap">

<span class="small text-muted">Gramos:</span>

<asp:TextBox
ID="txtGrams"
runat="server"
CssClass="form-control form-control-sm"
Style="max-width:120px"
Text="100" />

<asp:Button
ID="btnAdd"
runat="server"
CssClass="btn btn-success btn-sm"
Text="Agregar"
CommandName="add"
CommandArgument='<%# Eval("Id") %>' />

</div>

</div>
</div>

</ItemTemplate>

</asp:Repeater>

</div>


<script>

(function(){

var txt=document.getElementById('<%=txtSearch.ClientID%>');
var hf=document.getElementById('<%=hfFoodId.ClientID%>');
var box=document.getElementById('foodSuggestBox');

var debounce=null;

function hideBox(){
box.style.display="none";
box.innerHTML="";
}

function showItems(items){

if(!items || items.length===0){
hideBox();
return;
}

var html="";

for(var i=0;i<items.length;i++){

var it=items[i];

html+='<button type="button" class="list-group-item list-group-item-action" data-id="'+it.Id+'" data-name="'+it.Name+'">'+it.Name+'</button>';

}

box.innerHTML=html;
box.style.display="block";

}

function callSuggest(q){

fetch('<%=ResolveUrl("~/Pages/Food/FoodSearch.aspx/SearchFoods")%>', {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ q: q })
            })
                .then(r => r.json())
                .then(d => showItems(d.d))
                .catch(() => hideBox());

        }

        txt.addEventListener("input", function () {

            var q = (txt.value || "").trim();

            hf.value = "";

            if (q.length < 2) {
                hideBox();
                return;
            }

            if (debounce) clearTimeout(debounce);

            debounce = setTimeout(function () {
                callSuggest(q);
            }, 200);

        });


        box.addEventListener("click", function (e) {

            var btn = e.target.closest("button[data-id]");

            if (!btn) return;

            txt.value = btn.dataset.name;
            hf.value = btn.dataset.id;

            hideBox();

        });


        document.addEventListener("click", function (e) {

            if (e.target === txt || box.contains(e.target)) return;

            hideBox();

        });

    })();

</script>

</asp:Content>