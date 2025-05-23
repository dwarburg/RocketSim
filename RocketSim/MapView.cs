using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketSim;

// The map view scale: 1 pixel = 24,000 meters (24 km)
public class MapView(GraphicsDevice graphicsDevice, Texture2D pixel)
{
    private const float MetersPerPixel = 24000f;
    private const int RocketRadius = 10;
    private readonly Texture2D _rocketCircle = CreateRocketCircle(graphicsDevice);

    public bool IsMapViewActive { get; private set; }

    private static Texture2D CreateRocketCircle(GraphicsDevice graphicsDevice)
    {
        const int diameter = RocketRadius * 2;
        var rocketCircle = new Texture2D(graphicsDevice, diameter, diameter);
        var colorData = new Color[diameter * diameter];

        // Fill with white (you can improve this later with a circle alpha mask)
        for (var i = 0; i < colorData.Length; i++) colorData[i] = Color.White;

        rocketCircle.SetData(colorData);
        return rocketCircle;
    }

    public void OpenMapView()
    {
        IsMapViewActive = true;
    }

    public void CloseMapView()
    {
        IsMapViewActive = false;
    }

    public void Draw(SpriteBatch spriteBatch, RocketCurrentState rocketState, Planet planet,
        Texture2D earthMapViewTexture, SpriteFont font, OrbitElements orbitElements)
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
        var rocketPosPixels = screenCenter + new Vector2(rocketState.Position.X / MetersPerPixel,
            -rocketState.Position.Y / MetersPerPixel);
        spriteBatch.Draw(_rocketCircle, rocketPosPixels - new Vector2(RocketRadius), Color.White);

        if (orbitElements is not null)
            if (Physics.OrbitIsEllipse(orbitElements.Eccentricity))
            {
                // Convert periapsis and axes to pixel units BEFORE drawing
                var periapsisPixels = screenCenter + new Vector2(orbitElements.Periapsis.X / MetersPerPixel,
                    -orbitElements.Periapsis.Y / MetersPerPixel);
                var semiMajorAxisPixels = orbitElements.SemiMajorAxis / MetersPerPixel;
                var semiMinorAxisPixels = orbitElements.SemiMinorAxis / MetersPerPixel;

                // Draw the rocket orbit trajectory
                var trajectoryColor = Color.White;
                const float trajectoryThickness = 2f;
                const int trajectorySegments = 100;

                DrawEllipseFromFocusAndPeriapsis(
                    spriteBatch,
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
    public static void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color,
        float thickness)
    {
        var edge = end - start;
        var angle = (float)Math.Atan2(edge.Y, edge.X);
        spriteBatch.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
            null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    // Draw ellipse given the focus and periapsis, with axes lengths in pixels
    public void DrawEllipseFromFocusAndPeriapsis(
        SpriteBatch spriteBatch,
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
        var c = (float)Math.Sqrt(semiMajorAxisPixels * semiMajorAxisPixels - semiMinorAxisPixels * semiMinorAxisPixels);

        var majorAxisVector = periapsisPixels - focusPixels;
        var majorUnit = Vector2.Normalize(majorAxisVector);

        // Correct ellipse center position (center = focus - majorUnit * c)
        var center = focusPixels - majorUnit * c;

        var prevPoint = Vector2.Zero;
        var firstPoint = true;

        for (var i = 0; i <= segments; i++)
        {
            var theta = MathHelper.TwoPi * i / segments;

            // Parametric ellipse point relative to center
            var x = semiMajorAxisPixels * (float)Math.Cos(theta);
            var y = semiMinorAxisPixels * (float)Math.Sin(theta);

            // Rotate ellipse point to align with major axis direction
            var rotated = new Vector2(
                x * majorUnit.X - y * majorUnit.Y,
                x * majorUnit.Y + y * majorUnit.X
            );

            var point = center + rotated;

            if (!firstPoint) DrawLine(spriteBatch, pixel, prevPoint, point, color, thickness);
            prevPoint = point;
            firstPoint = false;
        }
    }
}