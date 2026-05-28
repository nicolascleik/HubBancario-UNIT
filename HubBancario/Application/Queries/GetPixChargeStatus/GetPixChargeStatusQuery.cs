using MediatR;

namespace HubBancario.Application.Queries.GetPixChargeStatus
{
    public class GetPixChargeStatusQuery : IRequest<string>
    {
        public string TxId { get; set; }
    }
}