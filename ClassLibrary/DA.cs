using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class DA
{
    public int ID { get; }
    public string Abteilung { get; set; }
    public string Titel { get; set; }
    public string Schueler { get; set; }

    public DA(int id, string abteilung, string titel, string schueler)
    {
        ID = id;
        Abteilung = abteilung;
        Titel = titel;
        Schueler = schueler;
    }

    public static bool SaveDAtoDb(List<DA> daList)
    {
        try
        {
            foreach (var da in daList)
            {
                string sql =
                    $"INSERT INTO DA (Abteilung, Titel, Schueler) VALUES (" +
                    $"'{da.Abteilung}', '{da.Titel}', '{da.Schueler}')";

                DbWrapper.Wrapper.RunNonQuery(sql);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveDAtoDb: {ex.Message}");
            return false;
        }
    }
}
