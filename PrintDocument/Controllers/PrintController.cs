using Microsoft.AspNetCore.Mvc;
using PrintDocument.Service;
using PrintDocument.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PrintDocument.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrintController : Controller
    {
        [HttpPost("surat-jalan/single-page/base64")]
        public IActionResult GenerateSinglePageSJ()
        {
            var doc = new SuratJalanDocument
            {
                SJNumber = "260000005/KR/SJ/I/2026",
                Recipient = "KURNIA",
                RecipientAddress = "KURNIA Jl. Cembul Rancamanyar KP. Cembul Pojol Rt/Rw. 02/16 Kel. Rancamanyar Kec. Baleendah-Bandung (terusan cibaduyut arah)",
                SubmissionDate = new DateOnly(2026, 1, 2),
                Armada = "B 9591 B",
                OrderDesc = "ORD/202512/6067 - Online DOUBLE 23",
                OPNumber = "25129103814",
                DONumber = "25129103814/XII/2025",
                DistributorName = "TBB",
                Ritase = 1,
                ExpedisiName = "BPAS - KRWG",
                WarehouseHead = "WAHYUDI",
                WarehouseStaff = "M. NUR HABIB",
                Items =
                [
                    new(1, "Keran Tembok AWET PVC JC 05 (Pieces)", 24, "PCS", "P3"),
                    new(2, "Keran Tembok AWET PVC JC 06 (Pieces)", 24, "PCS", "P3"),
                    new(3, "Kondom Keran AWET KK01 1/2 Inchi (Toples 40 Pieces)", 1, "PCS", "P3"),
                    new(4, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                ]
            };

            using var ms = new MemoryStream();

            // generate PDF to memory
            doc.GeneratePdf(ms);

            var pdfBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(pdfBytes);

            return Ok(new
            {
                fileName = "surat-jalan.pdf",
                contentType = "application/pdf",
                base64
            });
        }

        [HttpPost("surat-jalan/multi-page/base64")]
        public IActionResult GenerateMultiPageSJ()
        {
            var doc = new SJMultiPageDocument
            {
                SJNumber = "260000005/KR/SJ/I/2026",
                Recipient = "KURNIA",
                RecipientAddress = "KURNIA Jl. Cembul Rancamanyar KP. Cembul Pojol Rt/Rw. 02/16 Kel. Rancamanyar Kec. Baleendah-Bandung (terusan cibaduyut arah)",
                SubmissionDate = new DateOnly(2026, 1, 2),
                Armada = "B 9591 B",
                OrderDesc = "ORD/202512/6067 - Online DOUBLE 23",
                OPNumber = "25129103814",
                DONumber = "25129103814/XII/2025",
                DistributorName = "TBB",
                Ritase = 1,
                ExpedisiName = "BPAS - KRWG",
                WarehouseHead = "WAHYUDI",
                WarehouseStaff = "M. NUR HABIB",
                Items =
                [
                    new(1, "Keran Tembok AWET PVC JC 05 (Pieces)", 24, "PCS", "P3"),
                    new(2, "Keran Tembok AWET PVC JC 06 (Pieces)", 24, "PCS", "P3"),
                    new(3, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(4, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(5, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(6, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(7, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(8, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(9, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(10, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(11, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(12, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(13, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(14, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(15, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(16, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(17, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                    new(18, "Keran Tembok AWET PVC JC 05 (Pieces)", 1, "PCS", "P3"),
                ]
            };

            using var ms = new MemoryStream();

            // generate PDF to memory
            doc.GeneratePdf(ms);

            var pdfBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(pdfBytes);

            return Ok(new
            {
                fileName = "surat-jalan.pdf",
                contentType = "application/pdf",
                base64
            });
        }
    }
}
