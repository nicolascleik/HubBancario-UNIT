using System;
using System.Threading.Tasks;

namespace BankingHub.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyClientAsync(Guid clientId, object payload);
    }
}
