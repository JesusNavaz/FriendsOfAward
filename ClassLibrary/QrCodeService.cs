using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class QrCodeService
    {
        /// <summary>
        /// Erzeugt einen QR-Code als Base64-Data-URL (PNG)
        /// </summary>
        /// <param name="baseVoteUrl">Basis-URL z. B. https://intranet.htlvb.local/vote</param>
        /// <param name="token">Eindeutiger Benutzer-Token (GUID)</param>
        /// <param name="pixelsPerModule">Größe des QR-Codes (Standard 20)</param>
        /// <returns>Base64 PNG Data-URL</returns>
        public string CreateUserQrCode(
        string baseVoteUrl,
        Guid token,
        int pixelsPerModule = 20)
        {
            if (string.IsNullOrWhiteSpace(baseVoteUrl))
                throw new ArgumentException("BaseVoteUrl darf nicht leer sein");


            string payload = $"{baseVoteUrl.TrimEnd('/')}/{token}";


            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(data);


            byte[] pngBytes = qrCode.GetGraphic(pixelsPerModule);
            return $"data:image/png;base64,{Convert.ToBase64String(pngBytes)}";
        }
    }
}
