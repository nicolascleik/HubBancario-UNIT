namespace HubBancario.Application.DTOs
{
    public class WebhookEventDto
    {
        public string TxId { get; set; }
        public string Status { get; set; }
        public string RawPayload { get; set; }
    }
}
