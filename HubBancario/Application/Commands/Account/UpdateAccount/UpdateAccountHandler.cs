using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.Account.UpdateAccount
{
    public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id);
            
            if (account == null)
                throw new DomainException("A conta solicitada não foi encontrada.");

            account.UpdateDetails(request.BankId, request.AccountNumber, request.Agency);

            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.CommitAsync();
        }
    }
}