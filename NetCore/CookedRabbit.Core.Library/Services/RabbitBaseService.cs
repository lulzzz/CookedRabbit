using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Pools;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.LogStrings.GenericMessages;
using static CookedRabbit.Core.Library.Utilities.LogStrings.RabbitServiceMessages;

namespace CookedRabbit.Core.Library.Services
{
    /// <summary>
    /// CookedRabbit base service for building other services.
    /// </summary>
    public class RabbitBaseService
    {
        /// <summary>
        /// Base ILogger for derived services.
        /// </summary>
        protected ILogger _logger = null;

        /// <summary>
        /// Base IRabbitChannelPool for derived services.
        /// </summary>
        protected IRabbitChannelPool _rcp = null;

        /// <summary>
        /// Base RabbitSeasoning for derived services.
        /// </summary>
        protected RabbitSeasoning _seasoning = null; // Used for recovery later.

        /// <summary>
        /// Used for throttling.
        /// </summary>
        protected Random Rand = new Random();

        #region Error Handling Section

        /// <summary>
        /// Method for handling errors, flagging channels as dead, and loggin message.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="channelId"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task HandleError(Exception e, ulong channelId, params object[] args)
        {
            if (_rcp != null)
            {
                _rcp.FlagDeadChannel(channelId);
                var errorMessage = string.Empty;

                switch (e)
                {
                    case RabbitMQ.Client.Exceptions.AlreadyClosedException ace:
                        errorMessage = ClosedChannelMessage;
                        break;
                    case RabbitMQ.Client.Exceptions.RabbitMQClientException rabbies:
                        errorMessage = RabbitExceptionMessage;
                        break;
                    case Exception ex:
                        errorMessage = UnknownExceptionMessage;
                        break;
                    default: break;
                }

                if (_seasoning.WriteErrorsToILogger)
                {
                    if (_logger is null)
                    { await Console.Out.WriteLineAsync($"{NullLoggerMessage} Exception:{e.Message}"); }
                    else
                    { _logger.LogError(e, errorMessage, args); }
                }

                if (_seasoning.WriteErrorsToConsole)
                {
                    lock (Console.Out)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        Console.ResetColor();
                    }
                }
            }
        }

        #endregion

        #region Connection Section

        public void CloseConnections()
        {
            _rcp.CloseConnections();
        }

        #endregion
    }
}
