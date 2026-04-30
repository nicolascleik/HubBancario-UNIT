using System;

namespace HubBancario.Domain.ValueObjects
{
    public class Money
    {
        public decimal Value { get; init; }
        public string Currency { get; init; }

        public Money(decimal value, string currency = "BRL")
        {
            throw new NotImplementedException("Lógica de validação de moeda e valor pendente.");
        }
    }
}

