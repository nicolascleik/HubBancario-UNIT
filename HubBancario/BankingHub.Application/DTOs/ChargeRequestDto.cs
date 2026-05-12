using System;

namespace BankingHub.Application.DTOs
{
    public class ChargeRequestDto
    {
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string ClientDocument { get; set; }
        public string ClientName { get; set; }
        public string ExternalReference { get; set; }
    }
}
