using RabbitMQ.Client;
using System.Collections.Generic;

namespace CookedRabbit.Core.Library.Models
{
    public class AckableResult
    {
        public IModel Channel { get; set; }
        public List<BasicGetResult> Results { get; set; }
    }
}
