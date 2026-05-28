using HubBancario.Domain.Aggregates.PixCharge;

namespace HubBancario.Infrastructure.BankAdapters.Itau
{
    public static class ItauStatusMapper
    {
        public static PixChargeStatus MapItauStatus(string itauStatus)
        {
            // Aqui você mapeia o dicionário EXATO da documentação do Itaú para o nosso Enum
            return itauStatus.ToUpper() switch
            {
                "ATIVA" => PixChargeStatus.Active,
                "CONCLUIDA" => PixChargeStatus.Paid,
                "REMOVIDA_PELO_USUARIO_RECEBEDOR" => PixChargeStatus.Expired,
                "DEVOLVIDA" => PixChargeStatus.Expired,
                _ => PixChargeStatus.Active
            };
        }
    }
}