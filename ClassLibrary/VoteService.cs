using ClassLibrary;
using System;
using System.Collections.Generic;

public static class VoteService
{
    // Favoriten: +1, Superfavorit: +2
    // WorkNo ist die Button-Nummer (1..20), wird auf foa_diplomaworks.work_no gemappt.
    public static void SubmitVotes(string token, List<int> favoriten, int? superfavorit)
    {
        int qrId = QrCode.GetValidQrIdByToken(token);
        if (qrId <= 0)
            throw new Exception("QR-Code ungültig oder bereits verwendet.");

        if (favoriten.Count > 5)
            throw new Exception("Maximal 5 Favoriten erlaubt.");

        if (superfavorit.HasValue && favoriten.Contains(superfavorit.Value))
            throw new Exception("Superfavorit darf nicht zusätzlich als Favorit gesetzt sein.");

        // Favoriten +1
        foreach (int workNo in favoriten)
        {
            string sql =
                "INSERT INTO foa_diplomascores (diploma_work_id, points) " +
                $"SELECT id, 1 FROM foa_diplomaworks WHERE work_no = {workNo} " +
                "ON DUPLICATE KEY UPDATE points = points + 1, updated_at = CURRENT_TIMESTAMP";

            DbWrapperMySqlV2.Wrapper.RunNonQuery(sql);
        }

        // Superfavorit +2
        if (superfavorit.HasValue)
        {
            string sql =
                "INSERT INTO foa_diplomascores (diploma_work_id, points) " +
                $"SELECT id, 2 FROM foa_diplomaworks WHERE work_no = {superfavorit.Value} " +
                "ON DUPLICATE KEY UPDATE points = points + 2, updated_at = CURRENT_TIMESTAMP";

            DbWrapperMySqlV2.Wrapper.RunNonQuery(sql);
        }

        // QR sperren
        QrCode.MarkAsUsed(token);
    }
}
