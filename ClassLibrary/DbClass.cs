using System;
using System.Collections.Generic;
using System.Data;

public class FoA_DA
{
    public int Id { get; }
    public string PunkteSchueler { get; set; }
    public string PunkteVoting { get; set; }
    public string Schueler { get; set; }

    public FoA_DA(int id, string punkteSchueler, string punkteVoting, string schueler)
    {
        Id = id;
        PunkteSchueler = punkteSchueler;
        PunkteVoting = punkteVoting;
        Schueler = schueler;
    }

    // ---------------- SAVE foa_da ----------------

    public static bool SaveToFoaDa(List<FoA_DA> daten)
    {
        try
        {
            foreach (var da in daten)
            {
                string sql =
                    $"INSERT INTO foa_da (punkte_schueler, punkte_voting, Schueler) VALUES (" +
                    $"'{Escape(da.PunkteSchueler)}', " +
                    $"'{Escape(da.PunkteVoting)}', " +
                    $"'{Escape(da.Schueler)}')";

                DbWrapper.Wrapper.RunNonQuery(sql);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error SaveToFoaDa: {ex.Message}");
            return false;
        }
    }
}


public class Diplomarbeit
{
    public int Id { get; set; }
    public string Titel { get; set; }
    public int Punkte { get; set; }

    public Diplomarbeit(int id, string titel, int punkte)
    {
        Id = id;
        Titel = titel;
        Punkte = punkte;
    }

    // ---------------- SAVE diplomarbeiten ----------------

    public static bool SaveDiplomarbeiten(List<Diplomarbeit> arbeiten)
    {
        try
        {
            foreach (var da in arbeiten)
            {
                string sql =
                    $"INSERT INTO diplomarbeiten (Id, diplomarbeit, punkte) VALUES (" +
                    $"{da.Id}, " +
                    $"'{Escape(da.Titel)}', " +
                    $"{da.Punkte})";

                DbWrapper.Wrapper.RunNonQuery(sql);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error SaveDiplomarbeiten: {ex.Message}");
            return false;
        }
    }

    private static string Escape(string input)
    {
        return input.Replace("'", "''");
    }
}
