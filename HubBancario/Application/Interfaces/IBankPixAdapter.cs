using System.Threading.Tasks;
using HubBancario.Application.DTOs;

namespace HubBancario.Application.Interfaces
{
    public interface IBankPixAdapter
    {
        Task<ChargeResponseDto> GeneratePixAsync(ChargeRequestDto request);
        Task<string> CheckStatusAsync(string txId);
    }
}