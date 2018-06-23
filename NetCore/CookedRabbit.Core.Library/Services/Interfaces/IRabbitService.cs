using System.Threading.Tasks;

namespace CookedRabbit.Core.Library.Services
{
    public interface IRabbitService
    {
        Task<bool> PublishAsync(string queueName, byte[] payload);
    }
}
