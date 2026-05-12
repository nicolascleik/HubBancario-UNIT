using System.Threading.Tasks;

namespace BankingHub.Application.Interfaces
{
    public interface IMessageQueue
    {
        Task PublishAsync<T>(T message, string queueName);
    }
}
