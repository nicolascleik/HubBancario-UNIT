using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.ClientSecret.RevokeClientSecret
{
    public class RevokeClientSecretHandler : IRequestHandler<RevokeClientSecretCommand>
    {
        private readonly IClientSecretRepository _clientSecretRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeClientSecretHandler(IClientSecretRepository clientSecretRepository, IUnitOfWork unitOfWork)
        {
            _clientSecretRepository = clientSecretRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RevokeClientSecretCommand request, CancellationToken cancellationToken)
        {
            var clientSecret = await _clientSecretRepository.GetByIdAsync(request.Id);

            if (clientSecret == null)
                throw new DomainException("A credencial solicitada não foi encontrada.");

            clientSecret.Revoke();

            await _clientSecretRepository.UpdateAsync(clientSecret);
            await _unitOfWork.CommitAsync();
        }
    }
}