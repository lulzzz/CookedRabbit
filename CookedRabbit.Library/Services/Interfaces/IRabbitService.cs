using System.Threading.Tasks;

namespace CookedRabbit.Library.Services
{
    public interface IRabbitService
    {
        Task<bool> PublishAsync(string queueName, byte[] payload);
    }
}
