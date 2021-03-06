﻿namespace CookedRabbit.Core.Library.Models
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
        public string PingPongQueueName { get; set; } = "CookedRabbit.PingPong";

        /// <summary>
        /// Time (in ms) between Ping Pong message tests. Recommended value between 1000 and 15000ms.
        /// </summary>
        public int PingPongTime { get; set; } = 1000;

        /// <summary>
        /// Allows you to configure settings for connection the HTTP API Management on the RabbitMQ server.
        /// </summary>
        public ApiSeasoning ApiSettings { get; set; } = new ApiSeasoning();
    }
}
