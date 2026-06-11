using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Queries.PixKey
{
    public class GetPixKeyInfoHandler : IRequestHandler<GetPixKeyInfoQuery, PixKeyInfoDto>
    {
        private readonly IPixKeyRepository _pixKeyRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBankAdapterFactory _bankAdapterFactory;

        public GetPixKeyInfoHandler(
            IPixKeyRepository pixKeyRepository,
            IAccountRepository accountRepository,
            IBankAdapterFactory bankAdapterFactory)
        {
            _pixKeyRepository = pixKeyRepository;
            _accountRepository = accountRepository;
            _bankAdapterFactory = bankAdapterFactory;
        }

        public async Task<PixKeyInfoDto> Handle(GetPixKeyInfoQuery request, CancellationToken cancellationToken)
        {
            var pixKeyEntity = await _pixKeyRepository.GetByKeyValueAsync(request.KeyValue);
            if (pixKeyEntity == null) return null; // Retornar nulo fará a Controller devolver 404 Not Found

            var accountEntity = await _accountRepository.GetByIdAsync(pixKeyEntity.AccountId);
            if (accountEntity == null) return null;

            var adapter = _bankAdapterFactory.GetAdapter(accountEntity.BankId);

            return await adapter.GetPixKeyAsync(request.KeyValue);
        }
    }
}