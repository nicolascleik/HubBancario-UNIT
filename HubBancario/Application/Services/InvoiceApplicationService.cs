using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.CreateInvoice;
using HubBancario.Application.Commands.CreatePixCharge;
using HubBancario.Application.DTOs;
using HubBancario.Application.Queries.GetInvoice;
using MediatR;

namespace HubBancario.Application.Services
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