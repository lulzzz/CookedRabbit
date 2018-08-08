using System;
using System.Collections.Generic;
using System.Linq;

namespace CookedRabbit.Library.Utilities
{
    public static class Guard
    {
        public static void AgainstNull(object argumentValue, string argumentName)
        {
            if (argumentValue is null) throw new ArgumentNullException(argumentName);
        }

        public static void AgainstNullOrEmpty(string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue)) throw new ArgumentException($"{argumentName} can't be null or empty.");
        }

        public static void AgainstBothNullOrEmpty(string argumentValue, string argumentName, string secondArgumentValue, string secondArgumentName)
        {
            if (string.IsNullOrEmpty(argumentValue) && string.IsNullOrEmpty(secondArgumentValue)) throw new ArgumentException($"{argumentName} and {secondArgumentName} can't both be null or empty.");
        }

        public static void AgainstNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue is null || !argumentValue.Any()) throw new ArgumentException($"{argumentName} can't be null or empty.");
        }

        public static void AgainstTrue(bool argumentValue, string argumentName)
        {
            if (argumentValue) throw new ArgumentException($"{argumentName} can't be true for this method.");
        }

        public static void AgainstFalse(bool argumentValue, string argumentName)
        {
            if (!argumentValue) throw new ArgumentException($"{argumentName} can't be false for this method.");
        }
    }
}
