using System;
using System.Threading.Tasks;
using BankingHub.Application.Commands.CreateInvoice;
using BankingHub.Application.Commands.CreatePixCharge;
using BankingHub.Application.DTOs;
using BankingHub.Application.Queries.GetInvoice;
using MediatR;

namespace BankingHub.Application.Services
{
    public class InvoiceApplicationService
    {
        private readonly IMediator _mediator;

        public InvoiceApplicationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Guid> CreateInvoiceAsync(CreateInvoiceCommand command)
            => await _mediator.Send(command);

        public async Task<QrCodeResponseDto> CreatePixChargeAsync(CreatePixChargeCommand command)
            => await _mediator.Send(command);

        public async Task<InvoiceDto> GetInvoiceAsync(Guid invoiceId)
            => await _mediator.Send(new GetInvoiceQuery { InvoiceId = invoiceId });
    }
}
