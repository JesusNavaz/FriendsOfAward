using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClassLibrary
{
    public class QrCode
    {
        public int Id { get; set; }         // DB id
        public string Token { get; set; }   // token

        public QrCode(string token)
        {
            Token = token;
        }

        public static bool SaveQRtoDb(List<QrCode> qrCodes)
        {
            try
            {
                foreach (var qr in qrCodes)
                {
                    string tokenSafe = (qr.Token ?? "").Replace("'", "''");

                    string sql = $"INSERT INTO foa_qrcodes (token, is_used) VALUES ('{tokenSafe}', 0)";
                    DbWrapperMySqlV2.Wrapper.RunNonQuery(sql);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveQRtoDb: {ex.Message}");
                return false;
            }
        }

        // Liefert die QR-ID, aber nur wenn noch nicht used
        public static int GetValidQrIdByToken(string token)
        {
            string tokenSafe = (token ?? "").Replace("'", "''");

            string sql =
                $"SELECT id FROM foa_qrcodes WHERE token = '{tokenSafe}' AND is_used = 0 LIMIT 1";

            DataTable dt = DbWrapperMySqlV2.Wrapper.RunQuery(sql);
            if (dt == null || dt.Rows.Count == 0) return 0;

            return Convert.ToInt32(dt.Rows[0]["id"]);
        }

        public static void MarkAsUsed(string token)
        {
            string tokenSafe = (token ?? "").Replace("'", "''");
            string sql = $"UPDATE foa_qrcodes SET is_used = 1 WHERE token = '{tokenSafe}' AND is_used = 0";
            DbWrapperMySqlV2.Wrapper.RunNonQuery(sql);
        }
    }
}