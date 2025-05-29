using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace RocketSim;

public class GameStateManager
{
    private readonly InputManager _inputManager;
    private readonly MenuScreen _menuScreen;
    private readonly MapView _mapView;
    private readonly Planet _planet;
    private readonly RocketInitialProperties _rocketInitialProperties;
    private readonly Vector2 _rocketInitialPhysicsPosition;
    private OrbitElements _orbitElements;
    private readonly Func<RocketCurrentState> _getRocketCurrentState;

    public OrbitElements OrbitElements => _orbitElements;

    public GameStateManager(
        InputManager inputManager,
        MenuScreen menuScreen,
        MapView mapView,
        Func<RocketCurrentState> getRocketCurrentState,
        Planet planet,
        RocketInitialProperties rocketInitialProperties,
        Vector2 rocketInitialPhysicsPosition)
    {
        _inputManager = inputManager;
        _menuScreen = menuScreen;
        _mapView = mapView;
        _getRocketCurrentState = getRocketCurrentState;
        _planet = planet;
        _rocketInitialProperties = rocketInitialProperties;
        _rocketInitialPhysicsPosition = rocketInitialPhysicsPosition;
    }

    public void Update(GameTime gameTime, RocketSimGame game)
    {
        var keyboardState = Keyboard.GetState();

        _inputManager.Update(gameTime);

        // Menu toggle
        if (_inputManager.IsEscapePressed(keyboardState))
        {
            if (_menuScreen.IsMenuActive)
                _menuScreen.CloseCurrentMenu();
            else
                _menuScreen.OpenMenu();
        }

        // Map view toggle
        if (_inputManager.IsMPressed(keyboardState))
        {
            if (_mapView.IsMapViewActive)
                _mapView.CloseMapView();
            else
                _mapView.OpenMapView();
        }

        // Menu update or simulation update
        if (_menuScreen.IsMenuActive)
        {
            _menuScreen.Update(_getRocketCurrentState(), _rocketInitialPhysicsPosition, _rocketInitialProperties);
        }
        else
        {
            _getRocketCurrentState().Update(gameTime, keyboardState, _planet);
            _orbitElements = Physics.ComputeOrbit(_getRocketCurrentState().Position, _getRocketCurrentState().Velocity, _planet.Mass);
        }
    }
}