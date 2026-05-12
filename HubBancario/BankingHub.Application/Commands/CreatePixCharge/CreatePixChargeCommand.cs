using System;
using BankingHub.Application.DTOs;
using MediatR;

namespace BankingHub.Application.Commands.CreatePixCharge
{
    public class CreatePixChargeCommand : IRequest<QrCodeResponseDto>
    {
        public Guid InvoiceId { get; set; }
    }
}
