using System;
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
    public Vector2 Center { get; } = new(0, 0); //Flagging to change for coordinate change

    public static void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float distanceToSurface,
        int screenWidth,
        int screenHeight, RocketCurrentState rocketCurrentState, Texture2D earthSurfaceTexture)
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
            // Draw green planet surface

            // coordinate for monogame graphics start from the top left, positive y coordinates go down
            var verticalCoordinateOfSurfaceGraphic = (int)distanceToSurface + screenHeight / 2;

            //rotation in radians of the planet surface graphic (to approximate the circular planet surface)
            var rotation = 90f * (Math.PI / 180f) -
                           Math.Atan2(rocketCurrentState.Position.Y, rocketCurrentState.Position.X);

            // Draw the planet surface texture with rotation - spiteBatch CANNOT rotate 'Rectangle' objects - only 'sprites' 
            spriteBatch.Draw(
                earthSurfaceTexture,
                new Vector2(screenWidth / 2, verticalCoordinateOfSurfaceGraphic),
                null,
                Color.White,
                (float)rotation,
                new Vector2(screenWidth / 2, 0),
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}