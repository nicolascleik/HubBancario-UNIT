using HubBancario.Application.DTOs;
using HubBancario.Application.Interfaces;
using MediatR;

namespace HubBancario.Application.Queries.PixKey
{
    public class GetPixKeyInfoHandler : IRequestHandler<GetPixKeyInfoQuery, PixKeyInfoDto>
    {
        private readonly IBankPixAdapter _bankPixAdapter;

        public GetPixKeyInfoHandler(IBankPixAdapter bankPixAdapter)
        {
            _bankPixAdapter = bankPixAdapter;
        }

        public async Task<PixKeyInfoDto> Handle(GetPixKeyInfoQuery request, CancellationToken cancellationToken)
        {
            return await _bankPixAdapter.GetPixKeyAsync(request.KeyValue);
        }
    }
}
