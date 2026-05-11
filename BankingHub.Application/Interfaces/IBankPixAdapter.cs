using System.Threading.Tasks;
using BankingHub.Application.DTOs;

namespace BankingHub.Application.Interfaces
{
    public interface IBankPixAdapter
    {
        Task<ChargeResponseDto> GeneratePixAsync(ChargeRequestDto request);
        Task<string> CheckStatusAsync(string txId);
    }
}
