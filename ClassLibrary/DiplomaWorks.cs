using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class DiplomaWorks
{
    public int Id { get; set; }                 // DB id (AUTO_INCREMENT)
    public int WorkNo { get; set; }             // Nr (1..20)
    public string DepartmentCode { get; set; }  // MB, ME, ...
    public string Title { get; set; }           // Titel
    public string Authors { get; set; }         // Autor:innen

    public DiplomaWorks(int workNo, string departmentCode, string title, string authors)
    {
        WorkNo = workNo;
        DepartmentCode = departmentCode;
        Title = title;
        Authors = authors;
    }

    public static void ReplaceAllInDb(List<DiplomaWorks> daList)
    {
        // Vorgabe im Pflichtenheft: kompletter Ersatz
        // TRUNCATE geht NICHT wegen ForeignKey -> daher DELETE

        DbWrapperMySqlV2.Wrapper.RunNonQuery("DELETE FROM foa_diplomascores;");
        DbWrapperMySqlV2.Wrapper.RunNonQuery("DELETE FROM foa_diplomaworks;");

        // optional: IDs wieder bei 1 starten
        DbWrapperMySqlV2.Wrapper.RunNonQuery("ALTER TABLE foa_diplomaworks AUTO_INCREMENT = 1;");
        DbWrapperMySqlV2.Wrapper.RunNonQuery("ALTER TABLE foa_diplomascores AUTO_INCREMENT = 1;");

        SaveDAtoDb(daList);
    }


    public static bool SaveDAtoDb(List<DiplomaWorks> daList)
    {
        try
        {
            foreach (var da in daList)
            {
                string depSafe = (da.DepartmentCode ?? "").Replace("'", "''");
                string titleSafe = (da.Title ?? "").Replace("'", "''");
                string authorsSafe = (da.Authors ?? "").Replace("'", "''");

                string sql =
                    $"INSERT INTO foa_diplomaworks (work_no, department_code, title, authors) VALUES (" +
                    $"{da.WorkNo}, '{depSafe}', '{titleSafe}', '{authorsSafe}')";

                DbWrapperMySqlV2.Wrapper.RunNonQuery(sql);
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