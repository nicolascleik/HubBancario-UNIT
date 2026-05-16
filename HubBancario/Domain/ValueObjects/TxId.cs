using System;
using System.Text.RegularExpressions;
using HubBancario.Domain.Exceptions;

namespace HubBancario.Domain.ValueObjects
{
    public sealed record TxId
    {
        public string Value { get; }

        private TxId(string value)
        {
            Value = value;
        }

        public static TxId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("O TxId não pode ser vazio.");

            if (value.Length > 35)
                throw new DomainException("O TxId não pode exceder 35 caracteres conforme padrão BACEN.");

            if (!Regex.IsMatch(value, "^[a-zA-Z0-9]+$"))
                throw new DomainException("O TxId deve conter apenas caracteres alfanuméricos.");

            return new TxId(value);
        }

        public static TxId Generate(string prefix = "HUB")
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

            var generated = $"{prefix}{timestamp}{random}";

            if (generated.Length > 35)
                generated = generated.Substring(0, 35);

            return new TxId(generated);
        }

        public override string ToString() => Value;

        public static implicit operator string(TxId txId) => txId.Value;
    }
}