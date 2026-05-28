using System.Threading.Tasks;

namespace HubBancario.Application.Interfaces
{
    public interface IMessageQueue
    {
        Task PublishAsync<T>(T message, string queueName);
    }
}
