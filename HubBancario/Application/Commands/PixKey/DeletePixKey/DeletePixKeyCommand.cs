using System;
using MediatR;

namespace HubBancario.Application.Commands.PixKey.DeletePixKey
{
    // Retorna vazio (204 No Content) na API
    public class DeletePixKeyCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}