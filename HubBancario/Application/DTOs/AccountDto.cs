using System;

namespace HubBancario.Application.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Document { get; set; }
        public string BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Agency { get; set; }
        public bool IsActive { get; set; }
    }
}