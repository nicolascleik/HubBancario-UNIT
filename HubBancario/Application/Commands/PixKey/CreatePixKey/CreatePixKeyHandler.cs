using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.PixKey.CreatePixKey
{
    public class CreatePixKeyHandler : IRequestHandler<CreatePixKeyCommand, Guid>
    {
        private readonly IPixKeyRepository _pixKeyRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePixKeyHandler(
            IPixKeyRepository pixKeyRepository, 
            IAccountRepository accountRepository, 
            IUnitOfWork unitOfWork)
        {
            _pixKeyRepository = pixKeyRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreatePixKeyCommand request, CancellationToken cancellationToken)
        {
            // 1. Valida se a conta dona da chave realmente existe no banco
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new DomainException("A conta informada não existe ou não foi encontrada.");

            // 2. Instancia a entidade utilizando o Factory Method com as regras do Domínio
            var pixKey = HubBancario.Domain.Aggregates.Account.PixKey.Create(
                request.KeyValue, 
                request.AccountId
            );

            // 3. Persiste no repositório
            await _pixKeyRepository.AddAsync(pixKey);

            // 4. Efetiva a transação
            await _unitOfWork.CommitAsync();

            // 5. Retorna o ID gerado para a API montar a resposta (201 Created)
            return pixKey.Id;
        }
    }
}