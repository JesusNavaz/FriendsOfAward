using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class QrCode
    {
        public string QrId { get; set; }

        public QrCode(string qrId)
        {
            QrId = qrId;
        }

        public static bool SaveQRtoDb(List<QrCode> qrCodes)
        {
            try
            {
                foreach (var qr in qrCodes)
                {
                    string sql =
                        $"INSERT INTO QrCode (QrID) VALUES ('{qr.QrId}')";

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
}
