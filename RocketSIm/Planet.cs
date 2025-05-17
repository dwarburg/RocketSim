using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketSim;

public class Planet(float mass, float radius)
{
    public const float DefaultMass = 5.972e24f; // in kg
    public const float DefaultRadius = 6371000f; // in meters

    public float Mass { get; } = mass;
    public float Radius { get; } = radius;
    public Vector2 Center { get; } = new(0, -1 * DefaultRadius);

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float distanceToSurface, int screenWidth,
        int screenHeight)
    {
        //Draw Ground
        DrawGround(spriteBatch, graphicsDevice, distanceToSurface, screenWidth, screenHeight);

        //Draw Lower Atmosphere
        DrawLowerAtmosphere(spriteBatch, graphicsDevice, distanceToSurface, screenWidth, screenHeight);
    }

    public void DrawGround(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float distanceToSurface,
        int screenWidth, int screenHeight)
    {
        // Create a green texture
        var groundTexture = new Texture2D(graphicsDevice, screenWidth, screenHeight);
        var colorData = new Color[screenWidth * screenHeight];
        for (var i = 0; i < colorData.Length; i++) colorData[i] = Color.Green;
        groundTexture.SetData(colorData);

        // Calculate the rectangle for the planet surface
        var planetSurfaceHeight = screenHeight - (540 - distanceToSurface);
        var planetSurfaceRectangle = new Rectangle(0, (int)planetSurfaceHeight, screenWidth,
            screenHeight - (int)planetSurfaceHeight);

        // Draw the rectangle
        spriteBatch.Draw(groundTexture, planetSurfaceRectangle, Color.Green);
    }

    public void DrawLowerAtmosphere(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float distanceToSurface,
        int screenWidth, int screenHeight)
    {
        // Create a blue texture
        var lowerAtmosphereTexture = new Texture2D(graphicsDevice, screenWidth, screenHeight);
        var colorData = new Color[screenWidth * screenHeight];
        for (var i = 0; i < colorData.Length; i++) colorData[i] = Color.Blue;
        lowerAtmosphereTexture.SetData(colorData);

        // if planet surface is on screen, calculate the rectangle to start at planet surface and end at top of screen
        var planetSurfaceHeight = screenHeight - (540 - distanceToSurface);
        var planetSurfaceRectangle = new Rectangle(0, 0, screenWidth, (int)planetSurfaceHeight);

        // if planet surface is not on screen, draw the whole screen
        if (planetSurfaceHeight > screenHeight) planetSurfaceRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

        // Draw the rectangle
        spriteBatch.Draw(lowerAtmosphereTexture, planetSurfaceRectangle, Color.Blue);
    }
}