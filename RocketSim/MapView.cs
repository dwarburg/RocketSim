using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RocketSim
{
    // The map view scale: 1 pixel = 24,000 meters (24 km)
    public class MapView(GraphicsDevice graphicsDevice)
    {
        private const float MetersPerPixel = 24000f;

        public bool IsMapViewActive { get; private set; } = false;
        private readonly Texture2D pixel = CreatePixel(graphicsDevice);
        private static readonly int rocketRadius = 10;
        private readonly Texture2D rocketCircle = CreateRocketCircle(graphicsDevice);

        public float PeriapsisFloat { get; private set; } = 0f;
        public float ApoasisFloat { get; private set; } = 0f;

        private static Texture2D CreatePixel(GraphicsDevice graphicsDevice)
        {
            var pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        }

        private static Texture2D CreateRocketCircle(GraphicsDevice graphicsDevice)
        {
            var diameter = rocketRadius * 2;
            var rocketCircle = new Texture2D(graphicsDevice, diameter, diameter);
            var colorData = new Color[diameter * diameter];

            // Fill with white (you can improve this later with a circle alpha mask)
            for (int i = 0; i < colorData.Length; i++) colorData[i] = Color.White;

            rocketCircle.SetData(colorData);
            return rocketCircle;
        }

        public void OpenMapView() => IsMapViewActive = true;
        public void CloseMapView() => IsMapViewActive = false;

        public void Update(Game game, RocketCurrentState rocketState, Vector2 initialRocketPosition,
            RocketInitialProperties rocketInitialProperties)
        {
            // Intentionally left blank for now
        }

        public void Draw(SpriteBatch spriteBatch, RocketCurrentState rocketState, Planet planet, Texture2D earthMapViewTexture, SpriteFont font)
        {
            var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;
            var screenCenter = new Vector2(screenWidth / 2f, screenHeight / 2f);

            // Draw the planet map centered on screen
            spriteBatch.Draw(earthMapViewTexture,
                screenCenter,
                null,
                Color.White,
                0f,
                new Vector2(earthMapViewTexture.Width / 2f, earthMapViewTexture.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);

            // Draw rocket position on map (convert meters to pixels)
            Vector2 rocketPosPixels = screenCenter + new Vector2(rocketState.Position.X / MetersPerPixel, -rocketState.Position.Y / MetersPerPixel);
            spriteBatch.Draw(rocketCircle, rocketPosPixels - new Vector2(rocketRadius), Color.White);

            // Compute orbit elements
            OrbitElements orbitElements = Physics.ComputeOrbit(rocketState.Position, rocketState.Velocity, planet.Mass);

            PeriapsisFloat = orbitElements.Periapsis.Length() - planet.Radius;
            ApoasisFloat = orbitElements.Apoasis.Length() - planet.Radius;

            if (Physics.OrbitIsEllipse(orbitElements.Eccentricity))
            {
                // Convert periapsis and axes to pixel units BEFORE drawing
                Vector2 periapsisPixels = screenCenter + new Vector2(orbitElements.Periapsis.X / MetersPerPixel, -orbitElements.Periapsis.Y / MetersPerPixel);
                float semiMajorAxisPixels = orbitElements.SemiMajorAxis / MetersPerPixel;
                float semiMinorAxisPixels = orbitElements.SemiMinorAxis / MetersPerPixel;

                // Draw the rocket orbit trajectory
                var trajectoryColor = Color.White;
                var trajectoryThickness = 2f;
                var trajectorySegments = 100;

                DrawEllipseFromFocusAndPeriapsis(
                    spriteBatch,
                    pixel,
                    screenCenter,
                    periapsisPixels,
                    semiMajorAxisPixels,
                    semiMinorAxisPixels,
                    trajectorySegments,
                    trajectoryColor,
                    trajectoryThickness
                );
            }
        }

        // Draw a line between two points with thickness
        public static void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
                null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        // Draw ellipse given the focus and periapsis, with axes lengths in pixels
        public void DrawEllipseFromFocusAndPeriapsis(
            SpriteBatch spriteBatch,
            Texture2D pixel,
            Vector2 focusPixels,
            Vector2 periapsisPixels,
            float semiMajorAxisPixels,
            float semiMinorAxisPixels,
            int segments,
            Color color,
            float thickness
        )
        {
            // Calculate c = focal distance in pixels
            float c = (float)Math.Sqrt(semiMajorAxisPixels * semiMajorAxisPixels - semiMinorAxisPixels * semiMinorAxisPixels);

            Vector2 majorAxisVector = periapsisPixels - focusPixels;
            Vector2 majorUnit = Vector2.Normalize(majorAxisVector);

            // Correct ellipse center position (center = focus - majorUnit * c)
            Vector2 center = focusPixels - majorUnit * c;

            Vector2 prevPoint = Vector2.Zero;
            bool firstPoint = true;

            for (int i = 0; i <= segments; i++)
            {
                float theta = MathHelper.TwoPi * i / segments;

                // Parametric ellipse point relative to center
                float x = semiMajorAxisPixels * (float)Math.Cos(theta);
                float y = semiMinorAxisPixels * (float)Math.Sin(theta);

                // Rotate ellipse point to align with major axis direction
                Vector2 rotated = new Vector2(
                    x * majorUnit.X - y * majorUnit.Y,
                    x * majorUnit.Y + y * majorUnit.X
                );

                Vector2 point = center + rotated;

                if (!firstPoint)
                {
                    DrawLine(spriteBatch, pixel, prevPoint, point, color, thickness);
                }
                prevPoint = point;
                firstPoint = false;
            }
        }
    }
}
