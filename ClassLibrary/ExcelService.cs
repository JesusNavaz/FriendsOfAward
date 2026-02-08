using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

public static class ExcelServices
{
    // Import: Spalten A-D: Nr | AbteilungKürzel | Titel | Autor:innen (Header in Zeile 1)
    public static List<DiplomaWorks> ReadDiplomaworksFromExcel(Stream excelStream)
    {
        var list = new List<DiplomaWorks>();

        using var wb = new XLWorkbook(excelStream);
        var ws = wb.Worksheet(1);

        int row = 2;
        while (!ws.Cell(row, 1).IsEmpty())
        {
            int workNo = ws.Cell(row, 1).GetValue<int>();
            string dept = ws.Cell(row, 2).GetValue<string>();
            string title = ws.Cell(row, 3).GetValue<string>();
            string authors = ws.Cell(row, 4).GetValue<string>();

            list.Add(new DiplomaWorks(workNo, dept, title, authors));
            row++;
        }

        return list;
    }

    // Export: Join diplomaworks + diplomascores
    public static byte[] CreateResultsExcel()
    {
        string sql =
            "SELECT d.work_no, d.department_code, d.title, d.authors, IFNULL(s.points, 0) AS points " +
            "FROM foa_diplomaworks d " +
            "LEFT JOIN foa_diplomascores s ON s.diploma_work_id = d.id " +
            "ORDER BY points DESC, d.work_no ASC";

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
