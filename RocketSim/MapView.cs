using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows.Forms;

namespace RocketSim;

//the map view is approximately 1 pixel = 24,000 meters (24 km)

public class MapView(GraphicsDevice graphicsDevice)
{
    public bool IsMapViewActive { get; private set; } = false;
    private readonly Texture2D pixel = CreatePixel(graphicsDevice);
    private static readonly int rocketRadius = 10; 
    private readonly Texture2D rocketCircle = CreateRocketCircle(graphicsDevice);
    
    private static Texture2D CreatePixel(GraphicsDevice graphicsDevice)
    {
        var pixel = new Texture2D(graphicsDevice, 1, 1);
        var colorData = new Color[1];
        colorData[0] = Color.White;
        pixel.SetData(colorData);
        return pixel;
    }
    
    private static Texture2D CreateRocketCircle(GraphicsDevice graphicsDevice)
    {
        var rocketCircle = new Texture2D(graphicsDevice, rocketRadius * 2, rocketRadius * 2);
        var colorData = new Color[rocketRadius * rocketRadius * 4];
        for (var i = 0; i<colorData.Length; i++) colorData[i] = Color.White;
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

    public void Update(Game game, RocketCurrentState rocketState, Vector2 initialRocketPosition,
        RocketInitialProperties rocketInitialProperties)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, RocketCurrentState rocketState, Planet planet, Texture2D earthMapViewTexture, SpriteFont font)
    {
        var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
        var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;
        var screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);
        // Draw earth with position and origin at screenWidth/2, screenHeight/2
        spriteBatch.Draw(earthMapViewTexture,
            screenCenter,
            null,
            Color.White,
            0f,
            new Vector2(earthMapViewTexture.Width / 2f, earthMapViewTexture.Height / 2f),
            1f,
            SpriteEffects.None,
            0f);

        // Draw white square for rocket position on map
        var rocketPositionOnMapX = (screenWidth / 2) + (rocketState.Position.X / 24000);
        var rocketPositionOnMapY = (screenHeight / 2) - (rocketState.Position.Y / 24000);
        var rocketPositionOnMap = new Vector2(rocketPositionOnMapX, rocketPositionOnMapY);

        spriteBatch.Draw(rocketCircle, rocketPositionOnMap - new Vector2(rocketRadius, rocketRadius), Color.White);
        
        OrbitElements orbitElements = Physics.ComputeOrbit(rocketState.Position, rocketState.Velocity, planet.Mass);

        if (Physics.OrbitIsEllipse(orbitElements.Eccentricity))
        {
            // Draw the rocket orbit trajectory
            var trajectoryColor = Color.White;
            var trajectoryThickness = 2f;
            var trajectorySegments = 100; // Number of segments to draw the ellipse

            DrawEllipseFromFocusAndPeriapsis(
                spriteBatch,
                pixel,
                screenCenter,
                orbitElements.Periapsis,
                orbitElements.SemiMajorAxis,
                orbitElements.SemiMinorAxis,
                trajectorySegments,
                trajectoryColor,
                trajectoryThickness
            );

            // Display the Periapsis text
            var periapsisText =
                $"Periapsis: {orbitElements.Periapsis}";
            spriteBatch.DrawString(font, periapsisText, new Vector2(10, 150), Color.White);
        }
    }

    public static void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        spriteBatch.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    public void DrawEllipseFromFocusAndPeriapsis(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        Vector2 focus,
        Vector2 periapsis,
        float semiMajorAxis,
        float semiMinorAxis,
        int segments,
        Color color,
        float thickness
    )
    {
        float c = (float)Math.Sqrt(semiMajorAxis * semiMajorAxis - semiMinorAxis * semiMinorAxis);
        Vector2 majorAxisVector = periapsis - focus;
        Vector2 majorUnit = Vector2.Normalize(majorAxisVector);

        Vector2 center = focus + majorUnit * c; // ellipse center offset towards other focus

        float scale = 1f / 24000f; // meters to pixels

        Vector2 prev = Vector2.Zero;
        bool first = true;

        for (int i = 0; i <= segments; i++)
        {
            float theta = MathHelper.TwoPi * i / segments;

            float x = semiMajorAxis * (float)Math.Cos(theta);
            float y = semiMinorAxis * (float)Math.Sin(theta);

            Vector2 rotated = new Vector2(
                x * majorUnit.X - y * majorUnit.Y,
                x * majorUnit.Y + y * majorUnit.X
            );

            Vector2 point = center + rotated;

            // scale to pixels relative to screen center
            var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;
            var screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 pointPixels = screenCenter + (point - focus) * scale;

            if (!first)
            {
                DrawLine(spriteBatch, pixel, prev, pointPixels, color, thickness);
            }

            prev = pointPixels;
            first = false;
        }
    }


}