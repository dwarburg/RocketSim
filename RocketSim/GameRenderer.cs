using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RocketSim;

public class GameRenderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly GraphicsDeviceManager _graphics;
    private readonly TexturesAndFonts _textures;
    private readonly MenuScreen _menuScreen;
    private readonly MapView _mapView;
    private readonly RocketInitialProperties _rocketInitialProperties;
    private readonly Planet _planet;
    //Func delegeate always get latest state of rocket and orbit elements (not just the initial state at initialization)
    private readonly Func<RocketCurrentState> _getRocketState;
    private readonly Func<OrbitElements> _getOrbitElements;

    public GameRenderer(
        GraphicsDevice graphicsDevice,
        GraphicsDeviceManager graphics,
        TexturesAndFonts textures,
        MenuScreen menuScreen,
        MapView mapView,
        RocketInitialProperties rocketInitialProperties,
        Planet planet,
        Func<RocketCurrentState> getRocketState,
        Func<OrbitElements> getOrbitElements)
    {
        _graphicsDevice = graphicsDevice;
        _graphics = graphics;
        _textures = textures;
        _menuScreen = menuScreen;
        _mapView = mapView;
        _rocketInitialProperties = rocketInitialProperties;
        _planet = planet;
        _getRocketState = getRocketState;
        _getOrbitElements = getOrbitElements;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphicsDevice.Clear(Color.Black);

        spriteBatch.Begin();

        var rocketState = _getRocketState();
        var orbitElements = _getOrbitElements();

        if (_menuScreen.IsMenuActive)
        {
            _menuScreen.Draw(spriteBatch, _rocketInitialProperties);
        }
        else
        {
            if (_mapView.IsMapViewActive)
            {
                _mapView.Draw(spriteBatch, rocketState, _planet, _textures.EarthMapView, _textures.Font, orbitElements);
            }
            else
            {
                var rocketWindowPosition = new Vector2(
                    (float)_graphics.PreferredBackBufferWidth / 2,
                    (float)_graphics.PreferredBackBufferHeight / 2);

                var distanceToSurface = Vector2.Distance(rocketState.Position, _planet.Center) - _planet.Radius;

                Planet.Draw(
                    spriteBatch,
                    _graphicsDevice,
                    distanceToSurface,
                    _graphics.PreferredBackBufferWidth,
                    _graphics.PreferredBackBufferHeight,
                    rocketState,
                    _textures.EarthSurface);

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && rocketState.Fuel > 0)
                    rocketState.Draw(spriteBatch, _textures.Rocket, rocketWindowPosition);
                else
                    rocketState.Draw(spriteBatch, _textures.RocketNoFire, rocketWindowPosition);
            }

            if (_textures.Font != null)
            {
                DisplayText.DisplayTextOnScreen(spriteBatch, _textures.Font, rocketState, _planet, orbitElements);
            }
        }

        spriteBatch.End();
    }
}