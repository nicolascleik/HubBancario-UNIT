using System;

namespace HubBancario.Application.DTOs
{
    public class ChargeRequestDto
    {
        public string TxId { get; set; }
        public decimal Amount { get; set; }
        public string ChargeType { get; set; } // "COB" ou "COBV"
        public DateTime? DueDate { get; set; }
        public string ExternalReference { get; set; }
    }
}