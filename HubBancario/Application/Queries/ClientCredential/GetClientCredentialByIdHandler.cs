using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Queries.ClientCredential
{
    /// <summary>
    /// Handler responsável por processar a <see cref="GetClientCredentialByIdQuery"/>.
    /// Consulta o repositório de domínio e projeta o resultado para o DTO público.
    /// </summary>
    public class GetClientCredentialByIdHandler : IRequestHandler<GetClientCredentialByIdQuery, ClientCredentialDto>
    {
        private readonly IClientSecretRepository _clientSecretRepository;
        private readonly IMapper _mapper;

        public GetClientCredentialByIdHandler(
            IClientSecretRepository clientSecretRepository,
            IMapper mapper)
        {
            _clientSecretRepository = clientSecretRepository;
            _mapper = mapper;
        }

        public async Task<ClientCredentialDto> Handle(
            GetClientCredentialByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Busca o agregado de domínio pelo ID fornecido
            var clientSecret = await _clientSecretRepository.GetByIdAsync(request.Id);

            if (clientSecret == null)
                return null;

            // Projeta o agregado rico do DDD para o DTO simplificado que vai para a API
            return _mapper.Map<ClientCredentialDto>(clientSecret);
        }
    }
}
