using System;
using System.Threading;
using System.Threading.Tasks;
using BankingHub.Application.DTOs;
using BankingHub.Application.Interfaces;
using HubBancario.Domain.Repositories;
using MediatR;

namespace BankingHub.Application.Commands.CreatePixCharge
{
    public class CreatePixChargeHandler : IRequestHandler<CreatePixChargeCommand, QrCodeResponseDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IBankAdapterFactory _bankAdapterFactory;
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePixChargeHandler(
            IInvoiceRepository invoiceRepository,
            IClientRepository clientRepository,
            IBankAdapterFactory bankAdapterFactory,
            IPixChargeRepository pixChargeRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _bankAdapterFactory = bankAdapterFactory;
            _pixChargeRepository = pixChargeRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QrCodeResponseDto> Handle(CreatePixChargeCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
