using System;

namespace Domain.Extensions
{
    public static class FloatExtensions
    {
        private const int DIGITS = 2;
        public const float ONE_KG_IN_GRAMS = 1000;

        public static float ToKilogram(this float QtyInGrams)
        {
            return (float)Math.Round(QtyInGrams / ONE_KG_IN_GRAMS, DIGITS);
        }
    }
}
