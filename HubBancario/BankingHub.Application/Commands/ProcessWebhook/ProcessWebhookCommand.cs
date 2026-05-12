using MediatR;

namespace BankingHub.Application.Commands.ProcessWebhook
{
    public class ProcessWebhookCommand : IRequest
    {
        public string TxId { get; set; }
        public string RawPayload { get; set; }
    }
}
