using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Windows.Forms;

namespace RocketSim;

public class RocketSimGame : Game
{
    private const double DebounceDelay = 0.5; // Minimum delay (in seconds) between toggles
    private readonly GraphicsDeviceManager _graphics;
    private double _escapeDebounceTime; // Tracks the time since the last Escape key toggle
    private double _mDebounceTime; // Tracks the time since the last M key toggle
    private SpriteFont _font;
    private MenuScreen _menuScreen;
    private MapView _mapView;

    private Planet _planet;
    private RocketCurrentState _rocketCurrentState;
    private Texture2D _rocketTexture;
    private Texture2D _rocketTextureNoFire;
    private Texture2D _earthSurfaceTexture;
    private Texture2D _earthMapViewTexture;
    private SpriteBatch _spriteBatch;
    private readonly RocketInitialProperties _rocketInitialProperties = new();


    public RocketSimGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.IsFullScreen = true;
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        // Initialize the planet
        _planet = new Planet(Planet.DefaultMass, Planet.DefaultRadius);

        // Initialize the rocket
        var rocketInitialPhysicsPosition = new Vector2(0, _planet.Radius); //Flagging to change for coordinate change
        var rocketProperties = new RocketInitialProperties();
        _rocketCurrentState = new RocketCurrentState(rocketInitialPhysicsPosition, rocketProperties);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rocketTexture = Content.Load<Texture2D>("rocket");
        _rocketTextureNoFire = Content.Load<Texture2D>("rocketNoFire");
        _earthSurfaceTexture = Content.Load<Texture2D>("earthSurface");
        _earthMapViewTexture = Content.Load<Texture2D>("earthMapView");

        try
        {
            _font = Content.Load<SpriteFont>("DefaultFont");
        }
        catch
        {
            _font = null;
        }

        // Initialize the menu screen
        _menuScreen = new MenuScreen(_font, GraphicsDevice, this);

        // Initialize the map view
        _mapView = new MapView(GraphicsDevice);

    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        // Update the debounce timers (used to prevent menu flashing on and off)
        _escapeDebounceTime -= gameTime.ElapsedGameTime.TotalSeconds;
        _mDebounceTime -= gameTime.ElapsedGameTime.TotalSeconds;

        // Open or close menu
        if (keyboardState.IsKeyDown(Keys.Escape) && _escapeDebounceTime <= 0)
        {
            _menuScreen.OpenMenu();
            _escapeDebounceTime = DebounceDelay; // Reset the debounce timer
        }

        // Open or close Map View 
        if (keyboardState.IsKeyDown(Keys.M) && _mDebounceTime <= 0)
        {
            if (_mapView.IsMapViewActive)
            {
                _mapView.CloseMapView(); // Open the map view
            }
            else
            {
                _mapView.OpenMapView(); // Open the map view
            }
            _mDebounceTime = DebounceDelay; // Reset the debounce timer
        }

        if (_menuScreen.IsMenuActive)
        {
            _menuScreen.Update(this, _rocketCurrentState, new Vector2(0, 0),
                _rocketInitialProperties); // Pass initial rocket position
        } else
        {
            _rocketCurrentState.Update(gameTime, _planet.Center, _planet.Mass, keyboardState, _planet,
                _rocketTexture?.Height ?? 64);
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // Begin a drawing session for sprite batch class that draws graphics in MonoGame framework
        _spriteBatch.Begin();

        if (_menuScreen.IsMenuActive)
        {
            // Draw the menu
            _menuScreen.Draw(_spriteBatch, _rocketInitialProperties);
        }
        else
        {
            // Draw the map view if it's active
            if (_mapView.IsMapViewActive)
            {
                _mapView.Draw(_spriteBatch, _rocketCurrentState, _planet, _earthMapViewTexture, _font);
            }
            else
            {
                var rocketWindowPosition = new Vector2((float)_graphics.PreferredBackBufferWidth / 2,
                    (float)_graphics.PreferredBackBufferHeight / 2);

                var distanceToSurface = Vector2.Distance(_rocketCurrentState.Position, _planet.Center) - _planet.Radius;

                // Draw the planet surface and atmosphere
                Planet.Draw(_spriteBatch, GraphicsDevice, distanceToSurface, _graphics.PreferredBackBufferWidth, 
                    _graphics.PreferredBackBufferHeight, _rocketCurrentState, _earthSurfaceTexture);

                // Draw the rocket
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && _rocketCurrentState.Fuel > 0)
                    //if space key is pressed draw rocket, else draw rocket without fire
                    _rocketCurrentState.Draw(_spriteBatch, _rocketTexture, rocketWindowPosition);
                else
                    _rocketCurrentState.Draw(_spriteBatch, _rocketTextureNoFire, rocketWindowPosition);
            }
            

            // Display Text Values
            if (_font != null)
            {
                // Display the fuel 
                _spriteBatch.DrawString(_font, $"Fuel: {_rocketCurrentState.Fuel:F1}", new Vector2(10, 10),
                    Color.White);

                // Calculate and display the distance to the planet's center
                var distanceToCenter = Vector2.Distance(_rocketCurrentState.Position, _planet.Center);
                var distanceText = $"Distance to Planet Center: {distanceToCenter:F1} meters";
                _spriteBatch.DrawString(_font, distanceText, new Vector2(10, 30), Color.White);

                // Calculate and display the distance to the planet's surface
                var distanceToSurface = distanceToCenter - _planet.Radius;
                var distanceToSurfaceText = $"Altitude: {distanceToSurface:F1} meters";
                _spriteBatch.DrawString(_font, distanceToSurfaceText, new Vector2(10, 50), Color.White);

                // Display the rocket's position
                var rocketPositionText =
                    $"Position: X={_rocketCurrentState.Position.X:F1}, Y={_rocketCurrentState.Position.Y:F1}";
                _spriteBatch.DrawString(_font, rocketPositionText, new Vector2(10, 70), Color.White);

                // Display the rocket's velocity
                var rocketVelocityText =
                    $"Velocity: X={_rocketCurrentState.Velocity.X:F1}, Y={_rocketCurrentState.Velocity.Y:F1}";
                _spriteBatch.DrawString(_font, rocketVelocityText, new Vector2(10, 90), Color.White);

                // Display the rocket's acceleration
                var rocketAccelerationText =
                    $"Acceleration: X={_rocketCurrentState.Acceleration.X:F1}, Y={_rocketCurrentState.Acceleration.Y:F1}";
                _spriteBatch.DrawString(_font, rocketAccelerationText, new Vector2(10, 110), Color.White);

                // Display the rocket's total mass
                var rocketTotalMassText = $"Rocket Total Mass: {_rocketCurrentState.RocketTotalMass :F1} kg";
                _spriteBatch.DrawString(_font, rocketTotalMassText, new Vector2(10, 130), Color.White);
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}