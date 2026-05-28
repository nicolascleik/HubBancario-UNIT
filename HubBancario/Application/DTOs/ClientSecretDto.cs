using System;

namespace HubBancario.Application.DTOs
{
    public class ClientSecretDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public bool IsValid { get; set; }
    }
}