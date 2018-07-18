using CookedRabbit.Demo.Helpers;
using CookedRabbit.Library.Models;
using CookedRabbit.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CookedRabbit.Demo.DemoHelper;
using static CookedRabbit.Library.Utilities.Enums;

namespace CookedRabbit.Demo
{
    public class RabbitServiceSerializationExamples
    {
        #region RabbitService Setup

        private static readonly RabbitSeasoning _rabbitSeasoning = new RabbitSeasoning();
        private static RabbitSerializeService _rabbitSerializeService;
        private static readonly ZeroTestObject _testObject = new ZeroTestObject();
        private static readonly string _equalityCheck = string.Empty;

        #endregion

        #region RabbitService Serialize and Deserialize Test

        public static async Task RunRabbitServiceSerializeAndDeserializeTestAsync()
        {
            _rabbitSeasoning.PoolSettings.ChannelPoolCount = 1;
            _rabbitSeasoning.PoolSettings.ConnectionPoolCount = 1;
            _rabbitSeasoning.FactorySettings.EnableDispatchConsumersAsync = false;
            _rabbitSeasoning.SerializeSettings.SerializationMethod = SerializationMethod.ZeroFormat;
            _rabbitSerializeService = new RabbitSerializeService(_rabbitSeasoning);

            await RabbitService_SendMessageAsync();
            await RabbitService_ReceiveMessageAsync();
            await Console.Out.WriteLineAsync("Finished sending messages.");
        }

        public static async Task RabbitService_SendMessageAsync()
        {
            var envelope = new Envelope
            {
                ExchangeName = exchangeName,
                RoutingKey = queueName,
                MessageType = $"{ContentType.Textplain}{Charset.Utf8.Description()}"
            };

            _testObject.Name = "Donald Traitor Trump";
            _testObject.Address = "1 Moscow Lane";
            _testObject.BitsAndPieces = new List<string> { "Putin's Puppet." };

            await _rabbitSerializeService.SerializeAndPublishAsync(_testObject, envelope);
        }

        public static async Task RabbitService_ReceiveMessageAsync()
        {
            var result = await _rabbitSerializeService.GetAndDeserializeAsync<ZeroTestObject>(queueName);

            var doObjectsMatch = result.Name == _testObject.Name && result.Address == _testObject.Address && _testObject.BitsAndPieces.FirstOrDefault() == result.BitsAndPieces.FirstOrDefault();
            await Console.Out.WriteLineAsync($"Name, Address, and BitsAndPieces matched? {doObjectsMatch}");
        }

        #endregion
    }
}
