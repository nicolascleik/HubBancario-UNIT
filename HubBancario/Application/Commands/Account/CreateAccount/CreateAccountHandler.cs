using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.Commands.Account.CreateAccount
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. Instancia o Value Object (aqui ocorre a validação matemática do CPF/CNPJ)
            var document = Document.Create(request.Document);

            // 2. Instancia o Agregado através do método Factory para garantir integridade
            var account = HubBancario.Domain.Aggregates.Account.Account.Create(
                request.SecretId,
                document,
                request.BankId,
                request.AccountNumber,
                request.Agency
            );

            // 3. Adiciona no repositório em memória
            await _accountRepository.AddAsync(account);

            // 4. Salva no banco de dados consolidando a transação
            await _unitOfWork.CommitAsync();

            // 5. Retorna o ID gerado para a API
            return account.Id;
        }
    }
}