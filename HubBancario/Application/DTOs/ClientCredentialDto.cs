using System;

namespace HubBancario.Application.DTOs
{
   
    public class ClientCredentialDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public bool IsValid { get; set; }
    }
}
