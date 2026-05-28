namespace HubBancario.Application.DTOs
{
    public class ChargeResponseDto
    {
        public string TxId { get; set; }
        public string Emv { get; set; }
        public string QrCodeBase64 { get; set; }
        public string Status { get; set; }
    }
}
