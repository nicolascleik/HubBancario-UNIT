using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.UseCases.Invoices.GetInvoiceById
{
    /// <summary>
    /// Handler da query GetInvoiceByIdQuery.
    /// Busca a fatura no repositório e mapeia para InvoiceDto.
    /// </summary>
    public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPixChargeRepository _pixChargeRepository;

        public GetInvoiceByIdHandler(
            IInvoiceRepository invoiceRepository,
            IPixChargeRepository pixChargeRepository)
        {
            _invoiceRepository = invoiceRepository;
            _pixChargeRepository = pixChargeRepository;
        }

        public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
            if (invoice == null)
                throw new DomainException($"Invoice com Id '{request.InvoiceId}' não encontrada.");

            // Tenta enriquecer a resposta com os dados do PixCharge (se existir)
            string pixEmv = null;
            string pixTxId = null;
            string pixStatus = null;

            if (!string.IsNullOrWhiteSpace(invoice.ExternalReference))
            {
                var txId = new Domain.ValueObjects.TxId(invoice.ExternalReference);
                var pixCharge = await _pixChargeRepository.GetByTxIdAsync(txId);

                if (pixCharge != null)
                {
                    pixEmv = pixCharge.Emv;
                    pixTxId = pixCharge.TxId.Value;
                    pixStatus = pixCharge.Status.ToString();
                }
            }

            return new InvoiceDto
            {
                Id = invoice.Id,
                ClientId = invoice.ClientId,
                Amount = invoice.Amount.Value,
                Currency = invoice.Amount.Currency,
                DueDate = invoice.DueDate,
                Status = invoice.Status.ToString(),
                ExternalReference = invoice.ExternalReference,
                PixEmv = pixEmv,
                PixTxId = pixTxId,
                PixStatus = pixStatus
            };
        }
    }
}
