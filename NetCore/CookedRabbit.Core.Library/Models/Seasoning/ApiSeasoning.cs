namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// Class to fully season RabbitServices to your taste!
    /// </summary>
    public class ApiSeasoning
    {
        public bool RabbitApiAccessEnabled { get; set; } = true;

        public bool UseSsl { get; set; } = false;

        public string RabbitApiHostName { get; set; } = "localhost";

        public string RabbitApiUserName { get; set; } = "guest";

        public string RabbitApiUserPassword { get; set; } = "guest";

        public int RabbitApiPort { get; set; } = 15672;
    }
}
