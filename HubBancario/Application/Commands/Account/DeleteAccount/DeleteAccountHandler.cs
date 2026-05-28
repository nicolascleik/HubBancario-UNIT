using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.Account.DeleteAccount
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. Busca a conta no banco
            var account = await _accountRepository.GetByIdAsync(request.Id);

            if (account == null)
                throw new DomainException("A conta solicitada não foi encontrada.");

            // 2. Chama a regra de negócio no domínio (que mudará o status e validará se já não estava inativa)
            account.Deactivate();

            // 3. Informa ao repositório que o estado deste agregado mudou
            await _accountRepository.UpdateAsync(account);

            // 4. Efetiva a transação no banco de dados
            await _unitOfWork.CommitAsync();
        }
    }
}