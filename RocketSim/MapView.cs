using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

//the map view is approximately 1 pixel = 24,000 meters (24 km)

public class MapView()
{
    public bool IsMapViewActive { get; private set; } = false;
        
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

    public void Draw(SpriteBatch spriteBatch, RocketCurrentState rocketState, Planet planet, Texture2D earthMapViewTexture)
    {
        var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
        var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;

        // Draw the map view background
        // with position and origin at screenWidth/2, screenHeight/2
        spriteBatch.Draw(earthMapViewTexture, 
            new Vector2(screenWidth / 2, screenHeight / 2), 
            null, 
            Color.White, 
            0f,
            new Vector2(earthMapViewTexture.Width / 2f, earthMapViewTexture.Height / 2f),
            1f,
            SpriteEffects.None, 
            0f);

        //spriteBatch.Draw(earthMapViewTexture, new Vector2(0, 0), Color.White);

        // Draw white circle 10 pixels wide at screenWidth/2, screenHeight/4

        var rocketPositionOnMapX = (screenWidth / 2) + (rocketState.Position.X/24000);
        var rocketPositionOnMapY = (screenHeight /2) - (rocketState.Position.Y/24000);
        var rocketPositionOnMap = new Vector2(rocketPositionOnMapX, rocketPositionOnMapY);
        var rocketRadius = 10f; // Radius of the rocket circle in pixels
        var rocketCircle = new Texture2D(spriteBatch.GraphicsDevice, (int)rocketRadius * 2, (int)rocketRadius * 2);
        var colorData = new Color[(int)rocketRadius * (int)rocketRadius * 4];
        for (var i = 0; i < colorData.Length; i++) colorData[i] = Color.White;
        rocketCircle.SetData(colorData);
        spriteBatch.Draw(rocketCircle, rocketPositionOnMap - new Vector2(rocketRadius, rocketRadius), Color.White);

    }

}