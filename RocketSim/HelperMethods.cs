using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RocketSim
{
    internal class HelperMethods
    {

        public static Texture2D CreatePixel(GraphicsDevice graphicsDevice)
        {
            var pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        }


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
        
        public static bool IsValidFloatChar(char c)
        {
            return char.IsDigit(c) || c == '.' || c == '-';
        }

        public static char GetCharFromKey(Keys key, KeyboardState state)
        {
            switch (key)
            {
                case Keys.D0:
                case Keys.NumPad0: return '0';
                case Keys.D1:
                case Keys.NumPad1: return '1';
                case Keys.D2:
                case Keys.NumPad2: return '2';
                case Keys.D3:
                case Keys.NumPad3: return '3';
                case Keys.D4:
                case Keys.NumPad4: return '4';
                case Keys.D5:
                case Keys.NumPad5: return '5';
                case Keys.D6:
                case Keys.NumPad6: return '6';
                case Keys.D7:
                case Keys.NumPad7: return '7';
                case Keys.D8:
                case Keys.NumPad8: return '8';
                case Keys.D9:
                case Keys.NumPad9: return '9';
                case Keys.OemPeriod:
                case Keys.Decimal: return '.';
                case Keys.OemMinus:
                case Keys.Subtract: return '-';
                default: return '\0';
            }
        }
    }
}
