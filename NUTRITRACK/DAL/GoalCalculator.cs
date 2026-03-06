using System;
using System.Data;
using System.Data.SqlClient;

namespace NUTRITRACK.DAL
{
    public class UserGoals
    {
        public int Kcal { get; set; }
        public decimal Protein_g { get; set; }
        public decimal Carbs_g { get; set; }
        public decimal Fat_g { get; set; }
    }

    public static class GoalCalculator
    {
        public static UserGoals Calculate(int edad, decimal pesoKg, int alturaCm, string objetivo)
        {
            // BMR aproximado (sin sexo)
            double bmr = (10.0 * (double)pesoKg) + (6.25 * alturaCm) - (5.0 * edad) + 5.0;

            // actividad promedio
            double tdee = bmr * 1.45;

            string obj = (objetivo ?? "").Trim().ToLowerInvariant();

            double kcalTarget = tdee;

            if (obj.Contains("bajar"))
                kcalTarget = tdee - 400;
            else if (obj.Contains("subir"))
                kcalTarget = tdee + 300;
            else if (obj.Contains("rendimiento"))
                kcalTarget = tdee + 150;
            else
                kcalTarget = tdee;

            if (kcalTarget < 1400) kcalTarget = 1400;
            if (kcalTarget > 4200) kcalTarget = 4200;

            double proteinPerKg = 1.6;
            if (obj.Contains("bajar")) proteinPerKg = 2.0;
            if (obj.Contains("subir")) proteinPerKg = 1.8;
            if (obj.Contains("rendimiento")) proteinPerKg = 1.7;

            double fatPerKg = 0.8;
            if (obj.Contains("bajar")) fatPerKg = 0.7;
            if (obj.Contains("subir")) fatPerKg = 0.9;

            double protein_g = Math.Round((double)pesoKg * proteinPerKg, 1);
            double fat_g = Math.Round((double)pesoKg * fatPerKg, 1);

            double kcalFromProtein = protein_g * 4.0;
            double kcalFromFat = fat_g * 9.0;

            double remainingKcal = kcalTarget - (kcalFromProtein + kcalFromFat);
            if (remainingKcal < 200) remainingKcal = 200;

            double carbs_g = Math.Round(remainingKcal / 4.0, 1);

            return new UserGoals
            {
                Kcal = (int)Math.Round(kcalTarget, 0),
                Protein_g = (decimal)protein_g,
                Fat_g = (decimal)fat_g,
                Carbs_g = (decimal)carbs_g
            };
        }

        public static UserGoals GetOrBuildAndSaveGoals(int userId)
        {
            var existing = GetSavedGoals(userId);
            if (existing != null) return existing;

            var user = GetUserData(userId);
            var goals = Calculate(user.Edad, user.PesoKg, user.AlturaCm, user.Objetivo);
            UpsertGoals(userId, goals);
            return goals;
        }

        public static void RecalculateAndSave(int userId)
        {
            var user = GetUserData(userId);
            var goals = Calculate(user.Edad, user.PesoKg, user.AlturaCm, user.Objetivo);
            UpsertGoals(userId, goals);
        }

        private static UserGoals GetSavedGoals(int userId)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT TOP 1
  KcalObjetivo,
  ProteinaObjetivo,
  CarbsObjetivo,
  GrasaObjetivo
FROM dbo.MetasUsuario
WHERE UsuarioId = @UserId;", cn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new UserGoals
                    {
                        Kcal = Convert.ToInt32(r["KcalObjetivo"]),
                        Protein_g = Convert.ToDecimal(r["ProteinaObjetivo"]),
                        Carbs_g = Convert.ToDecimal(r["CarbsObjetivo"]),
                        Fat_g = Convert.ToDecimal(r["GrasaObjetivo"])
                    };
                }
            }
        }

        private static void UpsertGoals(int userId, UserGoals g)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.MetasUsuario WHERE UsuarioId = @UserId)
BEGIN
  UPDATE dbo.MetasUsuario
  SET
    KcalObjetivo = @Kcal,
    ProteinaObjetivo = @Prot,
    CarbsObjetivo = @Carb,
    GrasaObjetivo = @Fat,
    FechaActualizacion = GETDATE()
  WHERE UsuarioId = @UserId;
END
ELSE
BEGIN
  INSERT INTO dbo.MetasUsuario
    (UsuarioId, KcalObjetivo, ProteinaObjetivo, CarbsObjetivo, GrasaObjetivo)
  VALUES
    (@UserId, @Kcal, @Prot, @Carb, @Fat);
END;", cn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Kcal", SqlDbType.Int).Value = g.Kcal;

                var pProt = cmd.Parameters.Add("@Prot", SqlDbType.Decimal);
                pProt.Precision = 10; pProt.Scale = 1; pProt.Value = g.Protein_g;

                var pCarb = cmd.Parameters.Add("@Carb", SqlDbType.Decimal);
                pCarb.Precision = 10; pCarb.Scale = 1; pCarb.Value = g.Carbs_g;

                var pFat = cmd.Parameters.Add("@Fat", SqlDbType.Decimal);
                pFat.Precision = 10; pFat.Scale = 1; pFat.Value = g.Fat_g;

                cmd.ExecuteNonQuery();
            }
        }

        private class UserData
        {
            public int Edad { get; set; }
            public decimal PesoKg { get; set; }
            public int AlturaCm { get; set; }
            public string Objetivo { get; set; }
        }

        private static UserData GetUserData(int userId)
        {
            using (var cn = Db.Open())
            using (var cmd = new SqlCommand(@"
SELECT TOP 1
  Edad,
  PesoKg,
  AlturaCm,
  Objetivo
FROM dbo.Usuarios
WHERE Id = @UserId;", cn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        throw new Exception("No se encontró el usuario.");

                    int edad = (r["Edad"] == DBNull.Value) ? 0 : Convert.ToInt32(r["Edad"]);
                    decimal peso = (r["PesoKg"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["PesoKg"]);
                    int altura = (r["AlturaCm"] == DBNull.Value) ? 0 : Convert.ToInt32(r["AlturaCm"]);
                    string objetivo = Convert.ToString(r["Objetivo"] ?? "");

                    if (edad <= 0 || peso <= 0 || altura <= 0 || string.IsNullOrWhiteSpace(objetivo))
                        throw new Exception("Faltan datos del usuario (Edad, PesoKg, AlturaCm u Objetivo).");

                    return new UserData
                    {
                        Edad = edad,
                        PesoKg = peso,
                        AlturaCm = altura,
                        Objetivo = objetivo
                    };
                }
            }
        }
    }
}