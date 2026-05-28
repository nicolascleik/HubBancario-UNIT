using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Queries.PixKey
{
    public class GetPixKeysByAccountIdHandler : IRequestHandler<GetPixKeysByAccountIdQuery, IEnumerable<PixKeyDto>>
    {
        private readonly IPixKeyRepository _pixKeyRepository;
        private readonly IMapper _mapper;

        public GetPixKeysByAccountIdHandler(IPixKeyRepository pixKeyRepository, IMapper mapper)
        {
            _pixKeyRepository = pixKeyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PixKeyDto>> Handle(GetPixKeysByAccountIdQuery request, CancellationToken cancellationToken)
        {
            // Executa a busca assíncrona no repositório de domínio mapeando pelo ID da Conta
            var pixKeys = await _pixKeyRepository.GetByAccountIdAsync(request.AccountId);

            if (pixKeys == null)
                return Array.Empty<PixKeyDto>();

            // Converte a coleção de entidades ricas do domínio para uma lista de DTOs planos
            return _mapper.Map<IEnumerable<PixKeyDto>>(pixKeys);
        }
    }
}