using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookedRabbit.Demo.Helpers
{
    public class TestObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public InnerObject InnerObject { get; set; }
        public string RandoString { get; set; }
    }

    public class InnerObject
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
    }
}
