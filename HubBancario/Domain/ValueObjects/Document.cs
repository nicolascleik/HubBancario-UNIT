using System;
using System.Linq;
using HubBancario.Domain.Exceptions;

namespace HubBancario.Domain.ValueObjects
{
    public sealed record Document
    {
        public string Value { get; }

        // Propriedades auxiliares para saber o tipo do documento
        public bool IsCnpj => Value.Length == 14;
        public bool IsCpf => Value.Length == 11;

        // Construtor privado para garantir a criação apenas pelo método Factory
        private Document(string value)
        {
            Value = value;
        }

        public static Document Create(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                throw new DomainException("O documento não pode ser vazio.");

            var numericDocument = new string(document.Where(char.IsDigit).ToArray());

            if (numericDocument.Length != 11 && numericDocument.Length != 14)
                throw new DomainException("O documento deve conter 11 (CPF) ou 14 (CNPJ) dígitos válidos.");

            if (numericDocument.Length == 11 && !IsValidCpf(numericDocument))
                throw new DomainException("O CPF informado é inválido.");

            if (numericDocument.Length == 14 && !IsValidCnpj(numericDocument))
                throw new DomainException("O CNPJ informado é inválido.");

            return new Document(numericDocument);
        }

        private static bool IsValidCpf(string cpf)
        {
            if (cpf.All(c => c == cpf[0])) return false; // Bloqueia 111.111.111-11

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        private static bool IsValidCnpj(string cnpj)
        {
            if (cnpj.All(c => c == cnpj[0])) return false;

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        public override string ToString() => Value;
    }
}