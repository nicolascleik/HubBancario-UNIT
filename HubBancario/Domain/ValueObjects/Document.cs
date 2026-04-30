using System;

namespace HubBancario.Domain.ValueObjects
{
    public class Document
    {
        public string Value { get; init; }

        public Document(string value)
        {
            throw new NotImplementedException("Lógica de validação de CPF/CNPJ pendente.");
        }
    }
}

