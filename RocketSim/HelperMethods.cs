using Microsoft.Xna.Framework;
using System;

namespace RocketSim
{
    internal class HelperMethods
    {
        public static String ConvertMeterToKmIfAbove10k(float meters)
        {
            if (Math.Abs(meters) > 10000f)
            {
                return $"{(meters / 1000f):F0} Km";
            }
            else
            {
                return $"{meters} m";
            }
        }

        public static String ConvertMeterToKmIfAbove10k(Vector2 meters)
        {
            if (Math.Abs(meters.X) > 10000f || Math.Abs(meters.Y) > 10000f)
            {
                return $"X = {(meters.X / 1000f):F0} Km, Y={(meters.Y / 1000f):F0} Km";
            }
            else
            {
                return $"X = {(meters.X):F0} m, Y={(meters.Y):F0} m";
            }
        }
    }
}
