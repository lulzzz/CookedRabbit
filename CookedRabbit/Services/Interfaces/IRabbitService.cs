using System.Threading.Tasks;

namespace CookedRabbit.Services
{
    public interface IRabbitService
    {
        Task<bool> PublishAsync(string queueName, byte[] payload);
    }
}
