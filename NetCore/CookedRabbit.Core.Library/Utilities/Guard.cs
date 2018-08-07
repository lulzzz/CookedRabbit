using System;
using System.Collections.Generic;
using System.Linq;

namespace CookedRabbit.Core.Library.Utilities
{
    public static class Guard
    {
        public static void AgainstNull(object argumentValue, string argumentName)
        {
            if (argumentValue is null) throw new ArgumentNullException(argumentName);
        }

        public static void AgainstNullOrEmpty(string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue)) throw new ArgumentNullException(argumentName);
        }

        public static void AgainstNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue is null) throw new ArgumentNullException(argumentName);
            if (!argumentValue.Any()) throw new ArgumentException($"{argumentName} can't be empty");
        }
    }
}
