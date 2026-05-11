using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.UseCases.Clients.CreateClient
{
    /// <summary>
    /// Comando para registrar um novo cliente B2B no Hub.
    /// Cria o Client com seus dados cadastrais.
    /// </summary>
    public class CreateClientCommand : IRequest<ClientDto>
    {
        public string CompanyName { get; init; }

        /// <summary>CNPJ da empresa (validação feita pelo Value Object Document no Domínio).</summary>
        public string Document { get; init; }

        /// <summary>Identificador do banco parceiro padrão para este cliente (ex: "itau").</summary>
        public string DefaultBankId { get; init; }

        /// <summary>URL para envio do webhook de retorno ao ERP do cliente quando o Pix for pago.</summary>
        public string WebhookUrl { get; init; }
    }
}
