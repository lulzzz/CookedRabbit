using CookedRabbit.Core.Library.Models;
using CookedRabbit.Core.Library.Services;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CookedRabbit.Core.Library.Utilities.Enums;
using static CookedRabbit.Core.Demo.DemoHelper;
using System.Dynamic;
using Newtonsoft.Json;
using CookedRabbit.Core.Demo.Helpers;

namespace CookedRabbit.Core.Demo
{
    public class RabbitServiceCompressionExamples
    {
        #region RabbitService Setup

        private static readonly RabbitSeasoning _rabbitSeasoning = new RabbitSeasoning { RabbitHost = "localhost", ConnectionName = Environment.MachineName, EnableDispatchConsumersAsync = false };
        private static RabbitService _rabbitService;
        private static TestObject testObject = new TestObject();
        private static string _equalityCheck = string.Empty;

        #endregion

        #region RabbitService Compress and Decompress Test

        public static async Task RunRabbitServiceCompressAndDecompressTestAsync()
        {
            _rabbitSeasoning.EnableDispatchConsumersAsync = false;
            _rabbitService = new RabbitService(_rabbitSeasoning);

            await RabbitService_SendMessageAsync();
            await RabbitService_ReceiveMessageAsync();
            await Console.Out.WriteLineAsync("Finished sending messages.");
        }

        public static async Task RabbitService_SendMessageAsync()
        {
            var payload = await GetAnObjectAsBytes();

            await _rabbitService.CompressAndPublishAsync(exchangeName, queueName, payload, $"{ContentType.Json.Description()} {Charset.utf8.Description()}");
        }

        public static async Task RabbitService_ReceiveMessageAsync()
        {
            var result = await _rabbitService.GetAndDecompressAsync(queueName);

            var jsonResult = Encoding.UTF8.GetString(result);
            var receivedObject = JsonConvert.DeserializeObject<TestObject>(jsonResult);
            var equal = _equalityCheck == jsonResult;
            await Console.Out.WriteLineAsync($"Compressed message received. IsEqualToOriginal? {equal}");
        }

        private static async Task<byte[]> GetAnObjectAsBytes()
        {
            testObject.FirstName = "House";
            testObject.LastName = "Cat";
            testObject.InnerObject = new InnerObject { Test1 = "Test1", Test2 = "Test2" };
            testObject.RandoString = Encoding.UTF8.GetString(await GetRandomByteArray(1000));

            _equalityCheck = JsonConvert.SerializeObject(testObject);
            return Encoding.UTF8.GetBytes(_equalityCheck);
        }

        #endregion
    }
}
