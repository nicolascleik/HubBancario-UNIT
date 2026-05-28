using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.Queries.GetPixChargeStatus
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
            // Instancia o Value Object aplicando as regras e travas do BACEN
            var txIdVo = TxId.From(request.TxId);

            // Realiza a busca utilizando o tipo forte exigido pelo contrato do repositório
            var charge = await _pixChargeRepository.GetByTxIdAsync(txIdVo);

            if (charge == null)
                return "NOT_FOUND";

            // Retorna o estado atual em string (Active, Paid, Expired)
            return charge.Status.ToString();
        }
    }
}