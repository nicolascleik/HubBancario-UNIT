using System.Net.Http;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Application.Interfaces;

namespace HubBancario.Infrastructure.BankAdapters.Abstractions
{
    public abstract class BaseBankPixAdapter : IBankPixAdapter
    {
        protected readonly HttpClient _httpClient;

        protected BaseBankPixAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public abstract Task<ChargeResponseDto> GeneratePixAsync(ChargeRequestDto request);
        public abstract Task<string> CheckStatusAsync(string txId);
        public abstract Task<PixKeyInfoDto> GetPixKeyAsync(string keyValue);
    }
}
