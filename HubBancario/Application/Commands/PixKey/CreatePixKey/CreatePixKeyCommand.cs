using System;
using MediatR;

namespace HubBancario.Application.Commands.PixKey.CreatePixKey
{
    public class CreatePixKeyCommand : IRequest<Guid>
    {
        public string KeyValue { get; set; }
        public Guid AccountId { get; set; }
    }
}