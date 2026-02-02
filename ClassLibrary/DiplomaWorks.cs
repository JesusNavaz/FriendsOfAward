using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class DiplomaWorks
{
    public int Id { get; set; }                 
    public int WorkNo { get; set; }             
    public string DepartmentCode { get; set; }  
    public string Title { get; set; }           
    public string Authors { get; set; }         

    public DiplomaWorks(int workNo, string departmentCode, string title, string authors)
    {
        WorkNo = workNo;
        DepartmentCode = departmentCode;
        Title = title;
        Authors = authors;
    }

    public static bool SaveDAtoDb(List<DiplomaWorks> daList)
    {
        try
        {
            foreach (var da in daList)
            {
                // Minimal-Escaping für '
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