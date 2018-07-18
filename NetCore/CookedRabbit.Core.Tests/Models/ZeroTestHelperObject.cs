using System.Collections.Generic;
using ZeroFormatter;

namespace CookedRabbit.Core.Tests.Models
{
    [ZeroFormattable]
    public class ZeroTestHelperObject
    {
        [Index(0)]
        public virtual string Name { get; set; }
        [Index(1)]
        public virtual string Address { get; set; }
        [Index(2)]
        public virtual List<string> BitsAndPieces { get; set; }
    }
}
