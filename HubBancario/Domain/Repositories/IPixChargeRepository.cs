using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Repositories
{
    public interface IPixChargeRepository
    {
        // A busca de cobrança Pix geralmente é feita pelo TxId (chave do BACEN)
        Task<PixCharge> GetByTxIdAsync(TxId txId);
        Task AddAsync(PixCharge pixCharge);
        Task UpdateAsync(PixCharge pixCharge);
    }
}

