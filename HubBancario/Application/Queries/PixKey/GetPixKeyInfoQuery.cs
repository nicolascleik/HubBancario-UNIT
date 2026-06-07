using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.Queries.PixKey
{
    public class GetPixKeyInfoQuery : IRequest<PixKeyInfoDto>
    {
        public string KeyValue { get; set; } = string.Empty;
    }
}
