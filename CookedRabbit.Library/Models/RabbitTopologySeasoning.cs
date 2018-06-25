namespace CookedRabbit.Library.Models
{
    public class RabbitTopologySeasoning : RabbitSeasoning
    {
        // CookedRabbit RabbitTopologySeasoning Queue Settings

        // CookedRabbit RabbitTopologySeasoning Exchange Settings

        // CookedRabbit RabbitTopologyService Pool Settings
        public new ushort EmptyPoolWaitTime { get; set; } = 100; // milliseconds
        public new string RabbitHost { get; set; } = string.Empty;
        public new string LocalHostName { get; set; } = string.Empty;
        public new ushort ConnectionPoolCount { get; set; } = 2;
        public new ushort ChannelPoolCount { get; set; } = 10;
    }
}
