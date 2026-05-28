using HubBancario.Domain.Aggregates.PixCharge;

namespace HubBancario.Infrastructure.BankAdapters.Shared
{
    public static class StatusMappingExtensions
    {
        // Um contrato genérico que todo banco precisará implementar de alguma forma
        public static PixChargeStatus ToDomainStatus(this string bankStatus)
        {
            // Pode ser um switch genérico ou ser sobrescrito pelo mapper específico
            return bankStatus.ToUpper() switch
            {
                "ACTIVE" => PixChargeStatus.Active,
                "PAID" => PixChargeStatus.Paid,
                "EXPIRED" => PixChargeStatus.Expired,
                _ => PixChargeStatus.Active // Fallback seguro
            };
        }
    }
}