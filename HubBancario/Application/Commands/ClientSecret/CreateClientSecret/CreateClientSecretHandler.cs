using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Commands.ClientSecret.CreateClientSecret
{
    public class CreateClientSecretHandler : IRequestHandler<CreateClientSecretCommand, Guid>
    {
        private readonly IClientSecretRepository _clientSecretRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateClientSecretHandler(
            IClientSecretRepository clientSecretRepository,
            IUnitOfWork unitOfWork)
        {
            _clientSecretRepository = clientSecretRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateClientSecretCommand request, CancellationToken cancellationToken)
        {
            // O segredo agora é gerado de forma independente para que seu ID 
            // seja fornecido no método de fábrica Account.Create(...) posteriormente.
            var clientSecret = HubBancario.Domain.Aggregates.Account.ClientSecret.Create(
                request.AccountId, 
                request.SecretValue, 
                request.Certificate, 
                request.CertificatePassword
            );

            await _clientSecretRepository.AddAsync(clientSecret);
            await _unitOfWork.CommitAsync();

            return clientSecret.Id;
        }
    }
}