namespace HubBancario.Application.DTOs
{
    public class PixKeyInfoDto
    {
        public string KeyValue { get; set; } = string.Empty;
        public string? KeyType { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerDocument { get; set; }
        public string? BankName { get; set; }
        public string? BankId { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
}
