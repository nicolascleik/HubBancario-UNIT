using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Aggregates.Client;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.UseCases.Clients.CreateClient
{
    /// <summary>
    /// Handler do comando CreateClientCommand.
    /// Cria o Client, persiste e registra no AuditLog.
    /// </summary>
    public class CreateClientHandler : IRequestHandler<CreateClientCommand, ClientDto>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateClientHandler(
            IClientRepository clientRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            // O Value Object Document encapsula a validação de CNPJ
            var document = new Document(request.Document);

            // Cria a entidade Client via construtor do Domínio
            var client = new Client(
                companyName: request.CompanyName,
                document: document,
                defaultBankId: request.DefaultBankId,
                webhookUrl: request.WebhookUrl
            );

            await _clientRepository.AddAsync(client);

            // Auditoria imutável da criação
            var auditLog = new AuditLog(
                entityId: client.Id,
                action: AuditAction.EntityCreated,
                changes: $"Cliente '{client.CompanyName}' (CNPJ: {document.Value}) cadastrado no Hub."
            );
            await _auditLogRepository.AddAsync(auditLog);

            await _unitOfWork.CommitAsync();

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
