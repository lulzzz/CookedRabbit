using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookedRabbit.Library.Models
{
    public class AckableResult
    {
        public IModel Channel { get; set; }
        public List<BasicGetResult> Results { get; set; }
    }
}
