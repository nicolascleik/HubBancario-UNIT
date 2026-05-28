using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.Commands.CreateInvoice
{
    public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Guid>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvoiceHandler(
            IInvoiceRepository invoiceRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            // 1. Valida se a conta dona da fatura realmente existe
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null) throw new DomainException("Conta não encontrada.");

            // 2. Converte os dados primitivos em Value Objects ricos
            var moneyAmount = Money.BRL(request.Amount);

            // 3. Cria a entidade pelo Factory Method
            var invoice = Invoice.Create(account.Id, moneyAmount, request.DueDate, request.ExternalReference);

            // 4. Persiste no banco
            await _invoiceRepository.AddAsync(invoice);
            await _unitOfWork.CommitAsync();

            return invoice.Id;
        }
    }
}