using System;

namespace HubBancario.Domain.Exceptions
{
    /// <summary>
    /// Exceção base para todas as violações de regras de negócio da camada de Domínio.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

