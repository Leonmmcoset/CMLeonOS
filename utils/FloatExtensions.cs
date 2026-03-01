using System;

namespace CMLeonOS.Utils
{
    public static class FloatExtensions
    {
        public static float Map(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
