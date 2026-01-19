using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class FoA_DA
{
    public int ID { get; }
    public string Abteilung { get; set; }
    public string Titel { get; set; }
    public string Schueler { get; set; }

    public FoA_DA(int id, string abteilung, string titel, string schueler)
    {
        ID = id;
        Abteilung = abteilung;
        Titel = titel;
        Schueler = schueler;
    }

    public static bool SaveDAtoDb(List<FoA_DA> foaDA)
    {
        try
        {
            DisableForeignKeysAndTruncate();
            CreateTables();

            foreach (var da in foaDA)
            {
                string sql =
                    $"INSERT INTO FoA_DA (Abteilung, Titel, Schueler) VALUES (" +
                    $"'{Escape(da.Abteilung)}', '{Escape(da.Titel)}', '{Escape(da.Schueler)}')";

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

    // ---------------- QR-CODES ----------------

    public class FoA_QrCodes
    {
        public string QrId { get; set; }

        public FoA_QrCodes(string qrId)
        {
            QrId = qrId;
        }

        public static bool SaveQRtoDb(List<FoA_QrCodes> qrCodes)
        {
            try
            {
                DisableForeignKeysAndTruncate();
                CreateTables();

                foreach (var qr in qrCodes)
                {
                    string sql =
                        $"INSERT INTO FoA_QrCodes (QrID) VALUES ('{Escape(qr.QrId)}')";

                    DbWrapper.Wrapper.RunNonQuery(sql);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveQRtoDb: {ex.Message}");
                return false;
            }
        }
    }

    // ---------------- VOTING ----------------

    public class FoA_Voting_System
    {
        public int VotingId { get; }
        public string QrId { get; }
        public int DaId { get; }

        public FoA_Voting_System(int votingId, string qrId, int daId)
        {
            VotingId = votingId;
            QrId = qrId;
            DaId = daId;
        }
    }

    // ---------------- SQL STRUCTURE ----------------

    private static void DisableForeignKeysAndTruncate()
    {
        DbWrapper.Wrapper.RunNonQuery(
            "SET FOREIGN_KEY_CHECKS = 0;" +
            "TRUNCATE TABLE foa_da;" +
            "TRUNCATE TABLE foa_qrcodes;" +
            "TRUNCATE TABLE foa_voting_system;" +
            "SET FOREIGN_KEY_CHECKS = 1;");
    }

    private static bool CreateTables()
    {
        try
        {
            DbWrapper.Wrapper.RunNonQuery(
                "CREATE TABLE IF NOT EXISTS FoA_DA (" +
                "DaId INT NOT NULL AUTO_INCREMENT, " +
                "Abteilung VARCHAR(3) NOT NULL, " +
                "Titel VARCHAR(100) NOT NULL, " +
                "Schueler VARCHAR(200) NOT NULL, " +
                "PRIMARY KEY (DaId))");

            DbWrapper.Wrapper.RunNonQuery(
                "CREATE TABLE IF NOT EXISTS FoA_QrCodes (" +
                "QrID VARCHAR(8) NOT NULL, " +
                "PRIMARY KEY(QrID))");

            DbWrapper.Wrapper.RunNonQuery(
                "CREATE TABLE IF NOT EXISTS FoA_Voting_System (" +
                "VotingId INT NOT NULL AUTO_INCREMENT, " +
                "QrId VARCHAR(8) NOT NULL, " +
                "DaId INT NOT NULL, " +
                "PRIMARY KEY (VotingId), " +
                "FOREIGN KEY (QrId) REFERENCES FoA_QrCodes (QrId) ON DELETE CASCADE, " +
                "FOREIGN KEY (DaId) REFERENCES FoA_DA (DaId) ON DELETE CASCADE)");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateTables: {ex.Message}");
            return false;
        }
    }

    // ---------------- HELPERS ----------------

    public static List<string> UnUsedQrCodes()
    {
        var qrIds = new List<string>();

        DataTable table = DbWrapper.Wrapper.RunQuery(
            "SELECT QrId FROM FoA_QrCodes " +
            "WHERE QrId NOT IN (SELECT QrId FROM FoA_Voting_System)");

        foreach (DataRow row in table.Rows)
        {
            qrIds.Add(row["QrId"].ToString());
        }

        return qrIds;
    }

    private static string Escape(string input)
    {
        return input.Replace("'", "''");
    }
}
