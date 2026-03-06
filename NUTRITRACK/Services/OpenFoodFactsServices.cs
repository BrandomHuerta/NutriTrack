using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace NUTRITRACK.Services
{
    public class OpenFoodFactsService
    {
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(8)
        };

        // Tip rápido: buscamos algo común y tomamos un ejemplo
        public async Task<string> GetQuickTipAsync()
        {
            // Ejemplo: buscar "oats" y sacar 1 nombre para tip
            var url = "https://world.openfoodfacts.org/cgi/search.pl?search_terms=oats&search_simple=1&action=process&json=1&page_size=1";
            var json = await _http.GetStringAsync(url);

            var ser = new JavaScriptSerializer();
            var data = ser.Deserialize<Models.OpenFoodFactsSearchResponse>(json);

            if (data?.products != null && data.products.Length > 0)
            {
                var p = data.products[0];
                var name = string.IsNullOrWhiteSpace(p.product_name) ? "un alimento" : p.product_name;
                return $"Ejemplo: '{name}'. Usa el buscador para autocompletar macros.";
            }

            return "Tip: usa el buscador para autocompletar macros de alimentos.";
        }

        public async Task<Models.ApiFoodResult[]> SearchFoodsAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Array.Empty<Models.ApiFoodResult>();

            var q = Uri.EscapeDataString(term.Trim());
            var url = $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={q}&search_simple=1&action=process&json=1&page_size=8";

            var json = await _http.GetStringAsync(url);
            var ser = new JavaScriptSerializer();
            var data = ser.Deserialize<Models.OpenFoodFactsSearchResponse>(json);

            if (data?.products == null) return Array.Empty<Models.ApiFoodResult>();

            var results = new Models.ApiFoodResult[Math.Min(data.products.Length, 8)];
            for (int i = 0; i < results.Length; i++)
            {
                var p = data.products[i];
                results[i] = new Models.ApiFoodResult
                {
                    Name = string.IsNullOrWhiteSpace(p.product_name) ? "(Sin nombre)" : p.product_name,
                    CaloriesKcal_100g = p.nutriments.energy_kcal_100g,
                    Protein_100g = p.nutriments.proteins_100g,
                    Carbs_100g = p.nutriments.carbohydrates_100g,
                    Fat_100g = p.nutriments.fat_100g
                };
            }
            return results;
        }
    }
}