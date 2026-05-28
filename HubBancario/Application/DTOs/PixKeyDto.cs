using System;

namespace HubBancario.Application.DTOs
{
    public class PixKeyDto
    {
        public Guid Id { get; set; }
        public string KeyValue { get; set; }
        public Guid AccountId { get; set; }
        public bool IsActive { get; set; }
    }
}