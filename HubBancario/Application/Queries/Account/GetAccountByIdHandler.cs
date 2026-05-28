using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Queries.Account
{
    public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountByIdHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            // Busca o agregado rico do Domínio
            var account = await _accountRepository.GetByIdAsync(request.Id);

            if (account == null)
                return null;

            // Mapeia de forma segura para o DTO plano que vai para a API
            return _mapper.Map<AccountDto>(account);
        }
    }
}