using System;
using HubBancario.Domain.Exceptions;

namespace HubBancario.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Value { get; }
        public string Currency { get; }

        private Money(decimal value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public static Money BRL(decimal value)
        {
            if (value < 0)
                throw new DomainException("O valor monetário não pode ser negativo.");

            return new Money(Math.Round(value, 2), "BRL");
        }

        public static bool operator >(Money a, Money b) => a.Value > b.Value;
        public static bool operator <(Money a, Money b) => a.Value < b.Value;
        public static bool operator >=(Money a, Money b) => a.Value >= b.Value;
        public static bool operator <=(Money a, Money b) => a.Value <= b.Value;

        public override string ToString() => $"{Currency} {Value:N2}";
    }
}