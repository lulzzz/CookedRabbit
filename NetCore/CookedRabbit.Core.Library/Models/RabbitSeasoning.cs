namespace CookedRabbit.Core.Library.Models
{
    public class RabbitSeasoning
    {
        // CookedRabbit Bus Settings


        // CookedRabbit RabbitService Settings
        public ushort QosPrefetchSize { get; set; } = 0;
        public ushort QosPrefetchCount { get; set; } = 120;

        // CookedRabbit RabbitService Pool Settings
        public ushort EmptyPoolWaitTime { get; set; } = 100; // milliseconds
        public string RabbitHost { get; set; } = string.Empty;
        public string ConnectionName { get; set; } = string.Empty; // Used for giving the connections names
        public ushort ConnectionPoolCount { get; set; } = 4;
        public ushort ChannelPoolCount { get; set; } = 100;

        // RabbitMQ Channel Factory Settings
        public ushort MaxChannelsPerConnection { get; set; } = 1000;
        public ushort HeartbeatInterval { get; set; } = 10; // seconds
        public bool AutoRecovery { get; set; } = true;
        public bool TopologyRecovery { get; set; } = true;
        public ushort NetRecoveryTimeout { get; set; } = 10;
        public bool EnableDispatchConsumersAsync { get; set; } = false; // Set to True to be able to create AsyncConsumers
    }
}
