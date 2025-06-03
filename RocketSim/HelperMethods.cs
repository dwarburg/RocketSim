using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

internal class HelperMethods
{
    public static Texture2D CreatePixel(GraphicsDevice graphicsDevice)
    {
        var pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData([Color.White]);
        return pixel;
    }


    public static string ConvertMeterToKmIfAbove10K(float meters)
    {
        return Math.Abs(meters) > 10000f ? $"{meters / 1000f:F0} Km" : $"{meters} m";
    }

    public static string ConvertMeterToKmIfAbove10K(Vector2 meters)
    {
        if (Math.Abs(meters.X) > 10000f || Math.Abs(meters.Y) > 10000f)
            return $"X = {meters.X / 1000f:F0} Km, Y={meters.Y / 1000f:F0} Km";

        return $"X = {meters.X:F0} m, Y={meters.Y:F0} m";
    }

    public static bool IsValidFloatChar(char c)
    {
        return char.IsDigit(c) || c == '.' || c == '-';
    }

    public static char GetCharFromKey(Keys key, KeyboardState state)
    {
        return key switch
        {
            Keys.D0 or Keys.NumPad0 => '0',
            Keys.D1 or Keys.NumPad1 => '1',
            Keys.D2 or Keys.NumPad2 => '2',
            Keys.D3 or Keys.NumPad3 => '3',
            Keys.D4 or Keys.NumPad4 => '4',
            Keys.D5 or Keys.NumPad5 => '5',
            Keys.D6 or Keys.NumPad6 => '6',
            Keys.D7 or Keys.NumPad7 => '7',
            Keys.D8 or Keys.NumPad8 => '8',
            Keys.D9 or Keys.NumPad9 => '9',
            Keys.OemPeriod or Keys.Decimal => '.',
            Keys.OemMinus or Keys.Subtract => '-',
            _ => '\0'
        };
    }
}