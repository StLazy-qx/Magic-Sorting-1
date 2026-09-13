using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Scripts.Extensions
{
    public static class Guard
    {
        public static void NotNull(object argument, string argumentName)
        {
            if (argument is null)
                throw new ArgumentNullException(argumentName);
        }

        public static void NotNull(UnityEngine.Object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        public static void NotNullOrWhiteSpace(string argument, string argumentName)
        {
            NotNull(argument, argumentName);

            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException("Строка не может быть пустой " +
                    "или состоять только из пробелов.", argumentName);
            }
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> argument, string argumentName)
        {
            NotNull(argument, argumentName);

            if (argument.Any() == false)
            {
                throw new ArgumentException("Коллекция не может быть пустой.", argumentName);
            }
        }

        public static void Positive(int argument, string argumentName)
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argument,
                    "Значение должно быть положительным.");
            }
        }

        public static void NotNegative(int argument, string argumentName)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(
                    argumentName, argument, "Значение не должно быть отрицательным.");
            }
        }

        public static void InRange(int argument, int min, int max, string argumentName)
        {
            if (argument < min || argument > max)
            {
                throw new ArgumentOutOfRangeException(argumentName, argument,
                    $"Значение должно быть между {min} и {max} включительно.");
            }
        }

        public static void InRange(float argument, float min, float max, string argumentName)
        {
            if (argument < min || argument > max)
                throw new ArgumentOutOfRangeException(argumentName, argument, 
                    $"Значение должно быть между {min} и {max} включительно.");
        }

        public static void IsTrue(bool condition, string argumentName, string message)
        {
            if (condition == false)
                throw new ArgumentException(message, argumentName);
        }

        public static void IsTrue(bool condition, string message)
        {
            if (condition == false)
                throw new InvalidOperationException(message);
        }
    }
}
