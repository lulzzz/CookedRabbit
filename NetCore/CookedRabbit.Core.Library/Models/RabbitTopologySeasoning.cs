namespace CookedRabbit.Core.Library.Models
{
    public class RabbitTopologySeasoning : RabbitSeasoning
    {
        // CookedRabbit RabbitTopologyService Pool Settings
        public new ushort EmptyPoolWaitTime { get; set; } = 100; // milliseconds
        public new string RabbitHost { get; set; } = string.Empty;
        public new string ConnectionName { get; set; } = string.Empty;
        public new ushort ConnectionPoolCount { get; set; } = 1;
        public new ushort ChannelPoolCount { get; set; } = 10;
    }
}
