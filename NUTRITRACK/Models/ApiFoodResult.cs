using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUTRITRACK.Models
{
    public class ApiFoodResult
    {
        public string Name { get; set; }
        public decimal CaloriesKcal_100g { get; set; }
        public decimal Protein_100g { get; set; }
        public decimal Carbs_100g { get; set; }
        public decimal Fat_100g { get; set; }
    }

    public class OpenFoodFactsSearchResponse
    {
        public OpenFoodFactsProduct[] products { get; set; }
    }

    public class OpenFoodFactsProduct
    {
        public string product_name { get; set; }
        public OpenFoodFactsNutriments nutriments { get; set; }
    }

    public class OpenFoodFactsNutriments
    {
        public decimal energy_kcal_100g { get; set; }
        public decimal proteins_100g { get; set; }
        public decimal carbohydrates_100g { get; set; }
        public decimal fat_100g { get; set; }
    }
}