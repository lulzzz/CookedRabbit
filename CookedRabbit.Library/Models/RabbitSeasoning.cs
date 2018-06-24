namespace CookedRabbit.Library.Models
{
    public class RabbitSeasoning
    {
        // CookedRabbit Bus Settings


        // CookedRabbit Service Settings


        // CookedRabbit Pool Settings
        public string RabbitHost { get; set; } = string.Empty;
        public string LocalHostName { get; set; } = string.Empty;
        public ushort ConnectionPoolCount { get; set; } = 10;
        public ushort ChannelPoolCount { get; set; } = 100;

        // RabbitMQ Channel Factory Settings
        public ushort MaxChannelsPerConnection { get; set; } = 1000;
        public ushort HeartbeatInterval { get; set; } = 15;
        public bool AutoRecovery { get; set; } = true;
        public bool TopologyRecovery { get; set; } = true;
        public ushort NetRecoveryTimeout { get; set; } = 10;
        public bool EnableDispatchConsumersAsync { get; set; } = true;
    }
}
