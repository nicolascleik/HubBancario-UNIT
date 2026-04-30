using System;

namespace HubBancario.Domain.ValueObjects
{
    public class TxId
    {
        public string Value { get; init; }

        public TxId(string value)
        {
            throw new NotImplementedException("Lógica de validação alfanumérica do BACEN pendente.");
        }
    }
}

