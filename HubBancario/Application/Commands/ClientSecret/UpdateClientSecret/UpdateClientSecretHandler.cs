using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.ClientSecret.UpdateClientSecret
{
    public class UpdateClientSecretHandler : IRequestHandler<UpdateClientSecretCommand>
    {
        private readonly IClientSecretRepository _clientSecretRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateClientSecretHandler(IClientSecretRepository clientSecretRepository, IUnitOfWork unitOfWork)
        {
            _clientSecretRepository = clientSecretRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateClientSecretCommand request, CancellationToken cancellationToken)
        {
            var clientSecret = await _clientSecretRepository.GetByIdAsync(request.Id);

            if (clientSecret == null)
                throw new DomainException("A credencial solicitada não foi encontrada.");

            clientSecret.UpdateCertificate(request.Certificate, request.CertificatePassword);

            await _clientSecretRepository.UpdateAsync(clientSecret);
            await _unitOfWork.CommitAsync();
        }
    }
}