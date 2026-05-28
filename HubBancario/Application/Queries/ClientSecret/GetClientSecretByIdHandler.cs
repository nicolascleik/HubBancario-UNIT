using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Queries.ClientSecret
{
    public class GetClientSecretByIdHandler : IRequestHandler<GetClientSecretByIdQuery, ClientSecretDto>
    {
        private readonly IClientSecretRepository _clientSecretRepository;
        private readonly IMapper _mapper;

        public GetClientSecretByIdHandler(IClientSecretRepository clientSecretRepository, IMapper mapper)
        {
            _clientSecretRepository = clientSecretRepository;
            _mapper = mapper;
        }

        public async Task<ClientSecretDto> Handle(GetClientSecretByIdQuery request, CancellationToken cancellationToken)
        {
            // Busca a entidade de credenciais diretamente no repositório de domínio
            var clientSecret = await _clientSecretRepository.GetByIdAsync(request.Id);

            if (clientSecret == null)
                return null;

            // Mapeia de forma segura o agregado rico do DDD para o DTO simplificado
            return _mapper.Map<ClientSecretDto>(clientSecret);
        }
    }
}