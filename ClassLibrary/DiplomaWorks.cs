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
        DbWrapperMySqlV2.Wrapper.RunNonQuery("TRUNCATE TABLE foa_diplomaworks");
        DbWrapperMySqlV2.Wrapper.RunNonQuery("TRUNCATE TABLE foa_diplomascores");

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