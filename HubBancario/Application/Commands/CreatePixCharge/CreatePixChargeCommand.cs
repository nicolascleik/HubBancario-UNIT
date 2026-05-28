using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.Commands.CreatePixCharge
{
    public class CreatePixChargeCommand : IRequest<QrCodeResponseDto>
    {
        public Guid InvoiceId { get; set; }
    }
}
