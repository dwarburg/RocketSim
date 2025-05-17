using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketSim;

public class Planet(float mass, float radius)
{
    public const float DefaultMass = 5.972e24f; // in kg
    public const float DefaultRadius = 6371000f; // in meters
    public const float DefaultLowerAtmosphereHeight = 100000f; // in meters 

    public float Mass { get; } = mass;
    public float Radius { get; } = radius;
    public Vector2 Center { get; } = new(0, -1 * DefaultRadius);

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float distanceToSurface, int screenWidth,
        int screenHeight)
    {
        if (distanceToSurface < DefaultLowerAtmosphereHeight)
        {
            // Draw blue sky
            var lowerAtmosphereTexture = new Texture2D(graphicsDevice, screenWidth, screenHeight);
            var colorData = new Color[screenWidth * screenHeight];
            for (var i = 0; i < colorData.Length; i++) colorData[i] = Color.CornflowerBlue;
            lowerAtmosphereTexture.SetData(colorData);
            // Draw the rectangle
            spriteBatch.Draw(lowerAtmosphereTexture, new Rectangle(0, 0, screenWidth, screenHeight),
                Color.CornflowerBlue);
        }

        if (distanceToSurface < screenHeight)
        {
            // Draw green planet surface rectangle where the top border is at DefaultRadius
            var planetSurfaceTexture = new Texture2D(graphicsDevice, screenWidth, screenHeight);
            var colorData = new Color[screenWidth * screenHeight];
            for (var i = 0; i < colorData.Length; i++) colorData[i] = Color.Green;
            planetSurfaceTexture.SetData(colorData);
            // coordinate for monogame graphics start from the top left, positive y coordinates go down
            var verticalCoordinateOfSurfaceGraphic = (int)distanceToSurface + screenHeight / 2;
            spriteBatch.Draw(planetSurfaceTexture, new Rectangle(0, verticalCoordinateOfSurfaceGraphic, screenWidth, screenHeight),
                Color.Green);

        }
    }
}