using ClosedXML.Excel;
using System;
using System.Data;
using System.IO;

namespace ClassLibrary
{
    public static class ExcelService
    {
        // METHODEN

        // Import: Excel -> DB (ersetzt DB-Inhalt)
        // Excel-Spalten: Nr | AbteilungKürzel | Titel | Autor:innen  (Header in Zeile 1)
        public static void ImportDiplomaWorks(Stream excelStream)
        {
            // 1) Alte Daten löschen (Vorgabe Pflichtenheft)
            DbWrapperMySqlV2.Wrapper.RunNonQuery("TRUNCATE TABLE foa_diplomaworks;");
            DbWrapperMySqlV2.Wrapper.RunNonQuery("TRUNCATE TABLE foa_diplomascores;");

            // 2) Excel lesen
            using XLWorkbook wb = new XLWorkbook(excelStream);
            IXLWorksheet ws = wb.Worksheet(1);

            int row = 2; // Zeile 1 = Header
            while (!ws.Cell(row, 1).IsEmpty())
            {
                int workNo = ws.Cell(row, 1).GetValue<int>();
                string dept = ws.Cell(row, 2).GetValue<string>().Replace("'", "''");
                string title = ws.Cell(row, 3).GetValue<string>().Replace("'", "''");
                string authors = ws.Cell(row, 4).GetValue<string>().Replace("'", "''");

                // 3) In DB speichern
                string sql =
                    "INSERT INTO foa_diplomaworks (work_no, department_code, title, authors) VALUES " +
                    $"({workNo}, '{dept}', '{title}', '{authors}');";

                DbWrapperMySqlV2.Wrapper.RunNonQuery(sql);

                row++;
            }
        }

        // Export: DB -> Excel (Auswertung)
        public static byte[] ExportResultsToExcel()
        {
            string sql =
                "SELECT d.work_no, d.department_code, d.title, d.authors, IFNULL(s.points, 0) AS points " +
                "FROM foa_diplomaworks d " +
                "LEFT JOIN foa_diplomascores s ON s.diploma_work_id = d.id " +
                "ORDER BY points DESC, d.work_no ASC;";

            DataTable dt = DbWrapperMySqlV2.Wrapper.RunQuery(sql);

            using XLWorkbook wb = new XLWorkbook();
            IXLWorksheet ws = wb.AddWorksheet("Auswertung");

            // Header
            ws.Cell(1, 1).Value = "Nr";
            ws.Cell(1, 2).Value = "Abteilung";
            ws.Cell(1, 3).Value = "Titel";
            ws.Cell(1, 4).Value = "Autor:innen";
            ws.Cell(1, 5).Value = "Punkte";

            // Daten
            int r = 2;
            foreach (DataRow dr in dt.Rows)
            {
                ws.Cell(r, 1).Value = (XLCellValue)dr["work_no"];
                ws.Cell(r, 2).Value = (XLCellValue)dr["department_code"];
                ws.Cell(r, 3).Value = (XLCellValue)dr["title"];
                ws.Cell(r, 4).Value = (XLCellValue)dr["authors"];
                ws.Cell(r, 5).Value = (XLCellValue)dr["points"];
                r++;
            }

            ws.Columns().AdjustToContents();

            using MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
