using System;

namespace HubBancario.Domain.Exceptions
{
    /// <summary>
    /// Exceção padrão utilizada para representar violações de regras de negócio no Domínio.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException() : base() { }

        public DomainException(string message) : base(message) { }

        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}