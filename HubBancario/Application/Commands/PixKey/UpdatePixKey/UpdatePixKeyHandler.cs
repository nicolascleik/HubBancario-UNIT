using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.PixKey.UpdatePixKey
{
    public class UpdatePixKeyHandler : IRequestHandler<UpdatePixKeyCommand>
    {
        private readonly IPixKeyRepository _pixKeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePixKeyHandler(IPixKeyRepository pixKeyRepository, IUnitOfWork unitOfWork)
        {
            _pixKeyRepository = pixKeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdatePixKeyCommand request, CancellationToken cancellationToken)
        {
            // 1. Busca o agregado correspondente no banco de dados
            var pixKey = await _pixKeyRepository.GetByIdAsync(request.Id);

            if (pixKey == null)
                throw new DomainException("A chave Pix solicitada não foi encontrada.");

            // 2. Chama a regra de negócio do Domínio para aplicar a alteração com segurança
            pixKey.UpdateKey(request.NewKeyValue);

            // 3. Sinaliza para o Entity Framework que esta entidade foi modificada
            await _pixKeyRepository.UpdateAsync(pixKey);

            // 4. Salva a transação no banco de dados
            await _unitOfWork.CommitAsync();
        }
    }
}