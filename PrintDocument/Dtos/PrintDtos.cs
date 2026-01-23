namespace PrintDocument.Dtos
{
    public record SuratJalanItem(
        int No,
        string NamaBarang,
        int Jumlah,
        string Satuan,
        string Keterangan
    );
}
