using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.Account.ChangeAccountStatus
{
    public class ChangeAccountStatusHandler : IRequestHandler<ChangeAccountStatusCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeAccountStatusHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeAccountStatusCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id);

            if (account == null)
                throw new DomainException("A conta solicitada não foi encontrada.");

            account.ChangeStatus(request.IsActive);

            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.CommitAsync();
        }
    }
}