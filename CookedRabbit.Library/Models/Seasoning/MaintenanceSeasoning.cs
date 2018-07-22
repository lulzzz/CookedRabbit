namespace CookedRabbit.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class MaintenanceSeasoning
    {
        /// <summary>
        /// Turns on the Ping Pong test inside of Maintenance service.
        /// </summary>
        public bool EnablePingPong { get; set; } = true;

        /// <summary>
        /// Name of the queue to create and use for the Ping Pong test.
        /// </summary>
        public string PingPongQueueName { get; set; } = "CookedRabbit.Maintenance.PingPong";

        /// <summary>
        /// Time (in ms) between Ping Pong message tests. Recommended value between 1000 and 15000ms.
        /// </summary>
        public int PingPongTime { get; set; } = 1000;
    }
}
