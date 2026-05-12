using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.UseCases.Invoices.CreateInvoice
{
    /// <summary>
    /// Comando para criar uma nova fatura (Invoice) e gerar a cobrança Pix no banco parceiro.
    /// Recebido via POST /api/invoices pelo ERP do cliente.
    /// </summary>
    public class CreateInvoiceCommand : IRequest<InvoiceDto>
    {
        /// <summary>ID do cliente B2B que está solicitando a cobrança.</summary>
        public Guid ClientId { get; init; }

        /// <summary>Valor da cobrança em reais.</summary>
        public decimal Amount { get; init; }

        /// <summary>Data de vencimento da fatura.</summary>
        public DateTime DueDate { get; init; }

        /// <summary>Referência externa do ERP para rastreamento (opcional).</summary>
        public string ExternalReference { get; init; }
    }
}
