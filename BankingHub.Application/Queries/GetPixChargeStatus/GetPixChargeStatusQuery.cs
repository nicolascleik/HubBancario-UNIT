using MediatR;

namespace BankingHub.Application.Queries.GetPixChargeStatus
{
    public class GetPixChargeStatusQuery : IRequest<string>
    {
        public string TxId { get; set; }
    }
}
