using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.PixKey.DeletePixKey
{
    public class DeletePixKeyHandler : IRequestHandler<DeletePixKeyCommand>
    {
        private readonly IPixKeyRepository _pixKeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePixKeyHandler(IPixKeyRepository pixKeyRepository, IUnitOfWork unitOfWork)
        {
            _pixKeyRepository = pixKeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeletePixKeyCommand request, CancellationToken cancellationToken)
        {
            // 1. Busca a chave Pix pelo ID
            var pixKey = await _pixKeyRepository.GetByIdAsync(request.Id);

            if (pixKey == null)
                throw new DomainException("A chave Pix solicitada não foi encontrada.");

            // 2. Aciona o método de exclusão lógica (Soft Delete) no Domínio
            pixKey.Delete();

            // 3. Atualiza o estado da chave no repositório
            await _pixKeyRepository.UpdateAsync(pixKey);

            // 4. Salva no banco de dados e fecha a transação
            await _unitOfWork.CommitAsync();
        }
    }
}