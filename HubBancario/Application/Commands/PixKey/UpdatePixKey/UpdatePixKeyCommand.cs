using System;
using MediatR;

namespace HubBancario.Application.Commands.PixKey.UpdatePixKey
{
    public class UpdatePixKeyCommand : IRequest
    {
        public Guid Id { get; set; }
        public string NewKeyValue { get; set; }
    }
}