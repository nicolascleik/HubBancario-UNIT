namespace BankingHub.Application.DTOs
{
    public class QrCodeResponseDto
    {
        public string TxId { get; set; }
        public string Emv { get; set; }
        public string QrCodeBase64 { get; set; }
    }
}
