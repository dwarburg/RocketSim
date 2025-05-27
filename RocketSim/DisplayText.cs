using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RocketSim
{
    internal class DisplayText
    {
        public static void DisplayTextOnScreen(SpriteBatch _spriteBatch, SpriteFont font, RocketCurrentState rocketCurrentState, Planet planet, OrbitElements orbitElements)
        {
            var _periapsisFloat = orbitElements.Periapsis.Length() - planet.Radius;
            var _apoapsisFloat = orbitElements.Apoapsis.Length() - planet.Radius;

            // Display the fuel 
            _spriteBatch.DrawString(font, $"Fuel: {rocketCurrentState.Fuel:F1}", new Vector2(10, 10),
                Color.White);

            // Calculate and display the distance to the planet's center
            var distanceToCenter = Vector2.Distance(rocketCurrentState.Position, planet.Center);
            var distanceText =
                $"Distance to Planet Center: {HelperMethods.ConvertMeterToKmIfAbove10K(distanceToCenter)}";
            _spriteBatch.DrawString(font, distanceText, new Vector2(10, 30), Color.White);

            // Calculate and display the distance to the planet's surface
            var distanceToSurface = distanceToCenter - planet.Radius;
            var distanceToSurfaceText = $"Altitude: {HelperMethods.ConvertMeterToKmIfAbove10K(distanceToSurface)}";
            _spriteBatch.DrawString(font, distanceToSurfaceText, new Vector2(10, 50), Color.White);

            // Display the rocket's position
            var rocketPositionText =
                $"Position: {HelperMethods.ConvertMeterToKmIfAbove10K(rocketCurrentState.Position)}";
            _spriteBatch.DrawString(font, rocketPositionText, new Vector2(10, 70), Color.White);

            // Display the rocket's velocity
            var rocketVelocityText =
                $"Velocity: {HelperMethods.ConvertMeterToKmIfAbove10K(rocketCurrentState.Velocity)}";
            _spriteBatch.DrawString(font, rocketVelocityText, new Vector2(10, 90), Color.White);

            // Display the rocket's acceleration
            var rocketAccelerationText =
                $"Acceleration: {HelperMethods.ConvertMeterToKmIfAbove10K(rocketCurrentState.Acceleration)}";
            _spriteBatch.DrawString(font, rocketAccelerationText, new Vector2(10, 110), Color.White);

            // Display the rocket's total mass
            var rocketTotalMassText = $"Rocket Total Mass: {rocketCurrentState.RocketTotalMass:F1} kg";
            _spriteBatch.DrawString(font, rocketTotalMassText, new Vector2(10, 130), Color.White);

            //display apoapsis and periapsis
            var apoapsis = _apoapsisFloat;
            var periapsis = _periapsisFloat;
            var apoapsisText = $"Apoapsis: {HelperMethods.ConvertMeterToKmIfAbove10K(apoapsis)}";
            var periapsisText = $"Periapsis: {HelperMethods.ConvertMeterToKmIfAbove10K(periapsis)}";
            _spriteBatch.DrawString(font, apoapsisText, new Vector2(10, 150), Color.White);
            _spriteBatch.DrawString(font, periapsisText, new Vector2(10, 170), Color.White);
        }
    }
}
