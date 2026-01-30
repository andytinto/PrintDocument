using PrintDocument.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PrintDocument.Service
{
    public sealed class SJMultiPageDocument : IDocument
    {
        // ====== Inputs ======
        public string SJNumber { get; init; } = "";
        public string DONumber { get; set; } = string.Empty;
        public string OPNumber { get; set; } = string.Empty;
        public string Destination { get; init; } = "";
        public DateOnly SubmissionDate { get; init; }
        public DateOnly DeliveryDate { get; set; }
        public List<SuratJalanItem> Items { get; init; } = [];
        public string Recipient { get; set; } = string.Empty;
        public string RecipientAddress { get; set; } = string.Empty;
        public string OrderDesc { get; set; } = string.Empty;
        public string Armada { get; set; } = string.Empty;
        public string DistributorName { get; set; } = string.Empty;
        public string? WarehouseHead { get; set; }
        public string? WarehouseStaff { get; set; }
        public string ExpedisiName { get; set; } = string.Empty;
        public int Ritase { get; set; }


        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // ====== Layout Constants ======
        private const float DefaultFontSize = 9;
        private const float PageMargin = 20;

        private const int MaxItemRows = 11;
        private const float ItemRowHeight = 14;

        private const float ColNoWidth = 30;
        private const float ColJumlahWidth = 60;
        private const float ColKetWidth = 50;
        private const float ColP1P2Width = 60;

        private const float TableCellPaddingLeft = 4;

        // Signature layout
        private const float SignatureSpacerHeight = 24;   // ruang tanda tangan (untuk tulis)
        private const float SignatureLineHeight = 14;     // tinggi baris nama

        private const int RowsPerPage = 14;

        public void Compose(IDocumentContainer container)
        {
            var items = Items ?? [];
            var pages = Chunk(items, RowsPerPage);

            foreach (var pageItems in pages)
            {
                container.Page(page =>
                {
                    page.Size(Cm(21, 14));
                    page.Margin(PageMargin);
                    page.DefaultTextStyle(x => x.FontSize(DefaultFontSize));

                    // Header sama setiap halaman (kalau kamu mau)
                    page.Header().Element(ComposeHeaderContainer);

                    // Content: intro + table (tapi itemnya cuma 14 per page)
                    page.Content().Element(c => ComposeContentPerPage(c, pageItems));

                    // Footer: TTD sama di setiap halaman
                    page.Footer().Element(ComposeSignaturesContainer);
                });
            }
        }

        private void ComposeHeaderContainer(IContainer container)
        {
            container.Column(col =>
            {
                // ===== Header (SURAT JALAN) =====
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2.5f);
                    });

                    table.Cell().Element(CellNoBorder)
                        .Text("SURAT JALAN")
                        .AlignLeft()
                        .FontSize(16)
                        .Bold();

                    table.Cell().Element(CellNoBorder)
                        .AlignLeft()
                        .Text($"Dikirim ke: {Recipient}");

                    table.Cell().Element(CellNoBorder)
                        .AlignLeft()
                        .Text(text =>
                        {
                            text.Span($"No: {SJNumber}");
                            text.Span("\n");
                            text.Span(DistributorName);
                        });

                    table.Cell().Element(CellNoBorder)
                        .AlignLeft()
                        .Text(RecipientAddress);
                });

                // Spacing setelah header
                col.Item().PaddingBottom(10);

                // ===== Intro =====
                col.Item().Text("Dengan Hormat");
                col.Item().Text($"Harap diterima barang-barang dibawah ini sesuai DO No. {DONumber}");

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(3);
                    });

                    table.Cell().Element(CellNoBorder)
                        .AlignLeft()
                        .Text($"Tanggal: {SubmissionDate:dd/MM/yyyy}");

                    table.Cell().Element(CellNoBorder)
                        .AlignLeft()
                        .Text($"OP: {OPNumber}");

                    table.Cell().Element(CellNoBorder)
                        .AlignLeft()
                        .Text($"Tanggal Kirim: {DeliveryDate:dd/MM/yyyy}");
                });
            });
        }


        private void ComposeContentPerPage(IContainer container, IReadOnlyList<SuratJalanItem> pageItems)
        {
            container.Column(col =>
            {
                col.Item().PaddingTop(6);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(ColNoWidth);
                        columns.RelativeColumn();
                        columns.ConstantColumn(ColJumlahWidth);
                        columns.ConstantColumn(ColKetWidth);
                        columns.ConstantColumn(ColP1P2Width);
                    });

                    // Header tabel (akan ada di setiap halaman)
                    table.Header(header =>
                    {
                        header.Cell().Border(1).AlignCenter().Text("NO").Bold();
                        header.Cell().Border(1).AlignCenter().Text("JENIS & UKURAN BARANG").Bold();
                        header.Cell().Border(1).AlignCenter().Text("JUMLAH").Bold();
                        header.Cell().Border(1).AlignCenter().Text("KET").Bold();
                        header.Cell().Border(1).AlignCenter().Text("P1/P2/SISA").Bold();
                    });

                    foreach (var item in pageItems)
                    {
                        table.Cell().Element(CellBorderLR).AlignCenter().Text(item.No.ToString());
                        table.Cell().Element(CellBorderLR).AlignLeft().PaddingLeft(TableCellPaddingLeft).Text(item.NamaBarang);
                        table.Cell().Element(CellBorderLR).AlignRight().PaddingRight(TableCellPaddingLeft).Text(item.Jumlah.ToString());
                        table.Cell().Element(CellBorderLR).AlignCenter().Text(item.Satuan);
                        table.Cell().Element(CellBorderLR).AlignCenter().Text(item.Keterangan);
                    }

                    // Kalau kamu mau tabel “tinggi tetap” 14 rows tiap halaman:
                    var empty = Math.Max(0, RowsPerPage - pageItems.Count);
                    for (var i = 0; i < empty; i++)
                        AddEmptyRow(table);

                    //// Footer row (border bottom)
                    //table.Cell().Element(CellBorderLRB).Text(string.Empty);
                    //table.Cell().Element(CellBorderLRB)
                    //    .PaddingLeft(TableCellPaddingLeft)
                    //    .AlignLeft()
                    //    .Text(text =>
                    //    {
                    //        text.Span("Orderan Merchant: ");
                    //        text.Span(OrderDesc);
                    //        text.Span("\n");
                    //        text.Span("Expedisi: ").Bold();
                    //        text.Span(ExpedisiName).Bold();
                    //        text.Span(" Rit: ").Bold();
                    //        text.Span(Ritase.ToString()).Bold();
                    //    });

                    table.Cell().Element(CellBorderLRB).Text(string.Empty);
                    table.Cell().Element(CellBorderLRB).Text(string.Empty);
                    table.Cell().Element(CellBorderLRB).Text(string.Empty);
                });
            });
        }
        private void ComposeSignaturesContainer(IContainer container)
        {
            container.Column(col =>
            {
                // Kalimat penutup
                col.Item()
                    .Text("Atas perhatiannya kami ucapkan terima kasih");

                col.Item().PaddingTop(6);

                // Tabel tanda tangan
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    // Header
                    SignatureTitleCell(table, "Kepala Gudang");
                    SignatureTitleCell(table, "Staff Gudang");
                    SignatureTitleCell(table, "Pengemudi");
                    SignatureTitleCell(table, "Yang Menerima");

                    // Sub header / extra info
                    SignatureValueCell(table, string.Empty);
                    SignatureValueCell(table, string.Empty);
                    SignatureValueCell(table, Armada);
                    SignatureValueCell(table, string.Empty);

                    // Spacer area for handwriting
                    SignatureSpacerCell(table);
                    SignatureSpacerCell(table);
                    SignatureSpacerCell(table);
                    SignatureSpacerCell(table);

                    // PIC names
                    SignatureNameCell(table, WarehouseHead);
                    SignatureNameCell(table, WarehouseStaff);
                    SignatureNameCell(table, null);
                    SignatureNameCell(table, null);
                });
            });
        }




        private static IEnumerable<IReadOnlyList<T>> Chunk<T>(IReadOnlyList<T> source, int size)
        {
            for (var i = 0; i < source.Count; i += size)
            {
                var count = Math.Min(size, source.Count - i);
                var slice = new List<T>(count);
                for (var j = 0; j < count; j++)
                    slice.Add(source[i + j]);

                yield return slice;
            }
        }


        // ====== Helpers ======

        private static void SignatureTitleCell(TableDescriptor table, string title) =>
            table.Cell().Element(CellNoBorder).AlignCenter().Text(title);

        private static void SignatureValueCell(TableDescriptor table, string value) =>
            table.Cell().Element(CellNoBorder).AlignCenter().Text(value);

        private static void SignatureSpacerCell(TableDescriptor table) =>
            table.Cell().Element(CellNoBorder).Height(SignatureSpacerHeight).Text(string.Empty);

        private static void SignatureNameCell(TableDescriptor table, string? name)
        {
            // Format: ( Name )
            var safeName = string.IsNullOrWhiteSpace(name)
            ? "                                "
            : name.Trim();

            table.Cell().Element(CellNoBorder)
                .AlignCenter()
                .Height(SignatureLineHeight)
                .Text(text =>
                {
                    text.Span("( ");
                    text.Span(safeName);   // kosong = tetap ada ruang
                    text.Span(" )");
                });
        }

        private static IContainer CellNoBorder(IContainer c) => c.Border(0);

        private static IContainer CellBorderLR(IContainer c) =>
            c.BorderLeft(1).BorderRight(1).PaddingHorizontal(0).PaddingVertical(0);

        private static IContainer CellBorderLRB(IContainer c) =>
            c.BorderLeft(1).BorderRight(1).BorderBottom(1);

        private static void AddEmptyRow(TableDescriptor table)
        {
            table.Cell().Element(CellBorderLR).Height(ItemRowHeight).Text(string.Empty);
            table.Cell().Element(CellBorderLR).Height(ItemRowHeight).Text(string.Empty);
            table.Cell().Element(CellBorderLR).Height(ItemRowHeight).Text(string.Empty);
            table.Cell().Element(CellBorderLR).Height(ItemRowHeight).Text(string.Empty);
            table.Cell().Element(CellBorderLR).Height(ItemRowHeight).Text(string.Empty);
        }

        private const float PointsPerCm = 28.3464567f; // 72 / 2.54
        private static PageSize Cm(float w, float h) => new(w * PointsPerCm, h * PointsPerCm);
    }
}