using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class GameStateManager
{
    private readonly InputManager _inputManager;
    private readonly MenuScreen _menuScreen;
    private readonly MapView _mapView;
    private readonly RocketCurrentState _rocketCurrentState;
    private readonly Planet _planet;
    private readonly RocketInitialProperties _rocketInitialProperties;
    private readonly Vector2 _rocketInitialPhysicsPosition;
    private OrbitElements _orbitElements;

    public OrbitElements OrbitElements => _orbitElements;

    public GameStateManager(
        InputManager inputManager,
        MenuScreen menuScreen,
        MapView mapView,
        RocketCurrentState rocketCurrentState,
        Planet planet,
        RocketInitialProperties rocketInitialProperties,
        Vector2 rocketInitialPhysicsPosition)
    {
        _inputManager = inputManager;
        _menuScreen = menuScreen;
        _mapView = mapView;
        _rocketCurrentState = rocketCurrentState;
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
            _menuScreen.Update(game, _rocketCurrentState, _rocketInitialPhysicsPosition, _rocketInitialProperties);
        }
        else
        {
            _rocketCurrentState.Update(gameTime, keyboardState, _planet);
            _orbitElements = Physics.ComputeOrbit(_rocketCurrentState.Position, _rocketCurrentState.Velocity, _planet.Mass);
        }
    }
}