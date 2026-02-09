using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class ExcelService
    {
        public static async Task ImportDiplomaWorksAsync(Stream excelStream)
        {
            using var ms = new MemoryStream();
            await excelStream.CopyToAsync(ms);
            ms.Position = 0;
            using var wb = new XLWorkbook(ms);
            var ws = wb.Worksheet(1);
            var list = new List<DiplomaWorks>();
            int row = 2;

            while (!ws.Cell(row, 1).IsEmpty())
            {
                int workNo = ws.Cell(row, 1).GetValue<int>();
                string dept = ws.Cell(row, 2).GetString();
                string title = ws.Cell(row, 3).GetString();
                string authors = ws.Cell(row, 4).GetString();

                list.Add(new DiplomaWorks(workNo, dept, title, authors));
                row++;
            }

            var db = DbWrapperMySqlV2.Wrapper;

            db.RunNonQuery("DELETE FROM foa_diplomascores;");
            db.RunNonQuery("DELETE FROM foa_diplomaworks;");

            foreach (var d in list)
            {
                string deptSafe = d.DepartmentCode.Replace("'", "''");
                string titleSafe = d.Title.Replace("'", "''");
                string authorsSafe = d.Authors.Replace("'", "''");

                string sql =
                    "INSERT INTO foa_diplomaworks (work_no, department_code, title, authors) VALUES " +
                    $"({d.WorkNo}, '{deptSafe}', '{titleSafe}', '{authorsSafe}');";

                db.RunNonQuery(sql);
            }

            await Task.CompletedTask;
        }

        public static byte[] ExportResultsToExcel()
        {
            string sql =
                "SELECT d.work_no, d.department_code, d.title, d.authors, IFNULL(s.points, 0) AS points " +
                "FROM foa_diplomaworks d " +
                "LEFT JOIN foa_diplomascores s ON s.diploma_work_id = d.id " +
                "ORDER BY points DESC, d.work_no ASC;";

            DataTable dt = DbWrapperMySqlV2.Wrapper.RunQuery(sql);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Auswertung");

            ws.Cell(1, 1).Value = "Nr";
            ws.Cell(1, 2).Value = "Abteilung";
            ws.Cell(1, 3).Value = "Titel";
            ws.Cell(1, 4).Value = "Autor:innen";
            ws.Cell(1, 5).Value = "Punkte";

            int r = 2;
            foreach (DataRow dr in dt.Rows)
            {
                ws.Cell(r, 1).Value = Convert.ToInt32(dr["work_no"]);
                ws.Cell(r, 2).Value = Convert.ToString(dr["department_code"]) ?? "";
                ws.Cell(r, 3).Value = Convert.ToString(dr["title"]) ?? "";
                ws.Cell(r, 4).Value = Convert.ToString(dr["authors"]) ?? "";
                ws.Cell(r, 5).Value = Convert.ToInt32(dr["points"]);
                r++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}