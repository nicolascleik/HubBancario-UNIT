using System;
using System.Threading.Tasks;

namespace HubBancario.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyClientAsync(Guid accountId, object payload);
    }
}
