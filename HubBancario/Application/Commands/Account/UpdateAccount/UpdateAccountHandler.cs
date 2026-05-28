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
            // 1. Busca o agregado correspondente no repositório
            var account = await _accountRepository.GetByIdAsync(request.Id);
            
            if (account == null)
                throw new DomainException("A conta solicitada não foi encontrada.");

            // 2. Chama o método rico do Domínio (que vai checar se a conta está ativa, etc.)
            account.UpdateDetails(request.BankId, request.AccountNumber, request.Agency);

            // 3. Atualiza o estado rastreado no repositório
            await _accountRepository.UpdateAsync(account);

            // 4. Salva no banco de dados e fecha a transação
            await _unitOfWork.CommitAsync();
        }
    }
}