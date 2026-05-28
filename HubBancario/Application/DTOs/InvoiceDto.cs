using System;

namespace HubBancario.Application.DTOs
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string ExternalReference { get; set; }
    }
}