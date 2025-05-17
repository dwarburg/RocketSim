using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketSim;

public class Planet(float mass, float radius)
{
    public const float DefaultMass = 5.972e24f; // in kg
    public const float DefaultRadius = 6371000f; // in meters

    public float Mass { get; } = mass;
    public float Radius { get; } = radius;
    public Vector2 Center { get; } = new Vector2(0, -1 * Planet.DefaultRadius);
    
    private Texture2D _planetTexture;

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float distanceToSurface, int screenWidth, int screenHeight)
    {
        // Create a green texture
        var planetTexture = new Texture2D(graphicsDevice, screenWidth, screenHeight);
        var colorData = new Color[screenWidth * screenHeight];
        for (var i = 0; i < colorData.Length; i++)
        {
            colorData[i] = Color.Green;
        }
        planetTexture.SetData(colorData);

        // Calculate the rectangle for the planet surface
        var planetSurfaceHeight = screenHeight - (540 - distanceToSurface);
        var planetSurfaceRectangle = new Rectangle(0, (int)planetSurfaceHeight, screenWidth, screenHeight - (int)planetSurfaceHeight);

        // Draw the rectangle
        spriteBatch.Draw(planetTexture, planetSurfaceRectangle, Color.Green);
    }

}
