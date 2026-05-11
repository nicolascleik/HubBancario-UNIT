using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.UseCases.Clients.GetClientById
{
    /// <summary>
    /// Handler da query GetClientByIdQuery.
    /// Busca o cliente no repositório e mapeia para ClientDto.
    /// </summary>
    public class GetClientByIdHandler : IRequestHandler<GetClientByIdQuery, ClientDto>
    {
        private readonly IClientRepository _clientRepository;

        public GetClientByIdHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.GetByIdAsync(request.ClientId);
            if (client == null)
                throw new DomainException($"Cliente com Id '{request.ClientId}' não encontrado.");

            return new ClientDto
            {
                Id = client.Id,
                CompanyName = client.CompanyName,
                Document = client.Document.Value,
                IsActive = client.IsActive,
                DefaultBankId = client.DefaultBankId,
                WebhookUrl = client.WebhookUrl
            };
        }
    }
}
