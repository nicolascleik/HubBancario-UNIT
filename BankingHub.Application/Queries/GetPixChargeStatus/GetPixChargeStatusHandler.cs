using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Repositories;
using MediatR;

namespace BankingHub.Application.Queries.GetPixChargeStatus
{
    public class GetPixChargeStatusHandler : IRequestHandler<GetPixChargeStatusQuery, string>
    {
        private readonly IPixChargeRepository _pixChargeRepository;

        public GetPixChargeStatusHandler(IPixChargeRepository pixChargeRepository)
        {
            _pixChargeRepository = pixChargeRepository;
        }

        public async Task<string> Handle(GetPixChargeStatusQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
