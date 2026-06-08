using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Application.Interfaces;
using MediatR;

namespace HubBancario.Application.Queries.PixKey
{
    public class GetPixKeyInfoHandler : IRequestHandler<GetPixKeyInfoQuery, PixKeyInfoDto>
    {
        private readonly IBankAdapterFactory _bankAdapterFactory;

        public GetPixKeyInfoHandler(IBankAdapterFactory bankAdapterFactory)
        {
            _bankAdapterFactory = bankAdapterFactory;
        }

        public async Task<PixKeyInfoDto> Handle(GetPixKeyInfoQuery request, CancellationToken cancellationToken)
        {
            var adapter = _bankAdapterFactory.GetAdapter("ITAU");

            return await adapter.GetPixKeyAsync(request.KeyValue);
        }
    }
}
