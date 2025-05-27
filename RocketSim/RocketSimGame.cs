using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class RocketSimGame : Game
{
    private const double DebounceDelay = 0.5; // Minimum delay (in seconds) between toggles
    private readonly GraphicsDeviceManager _graphics;
    private readonly RocketInitialProperties _rocketInitialProperties = new();
    private Texture2D _earthMapViewTexture;
    private Texture2D _earthSurfaceTexture;
    private EditRocketPropertiesScreen _editRocketPropertiesScreen;
    private double _escapeDebounceTime; // Tracks the time since the last Escape key toggle
    private SpriteFont _font;
    private MapView _mapView;
    private double _mDebounceTime; // Tracks the time since the last M key toggle
    private MenuScreen _menuScreen;

    private Texture2D _pixel;

    private Planet _planet;
    public RocketCurrentState RocketCurrentState;
    private Texture2D _rocketTexture;
    private Texture2D _rocketTextureNoFire;
    private SpriteBatch _spriteBatch;

    private OrbitElements _orbitElements;

    public Vector2 RocketInitialPhysicsPosition;

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
        RocketInitialPhysicsPosition = new Vector2(0, _planet.Radius);
        var rocketProperties = new RocketInitialProperties();
        RocketCurrentState = new RocketCurrentState(RocketInitialPhysicsPosition, rocketProperties);

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

        //Initialize the pixel texture for drawing
        _pixel = HelperMethods.CreatePixel(GraphicsDevice);

        // Initialize the edit rocket properties screen
        _editRocketPropertiesScreen = new EditRocketPropertiesScreen(_font, _pixel, _rocketInitialProperties, GraphicsDevice, this);

        // Initialize the menu screen
        _menuScreen = new MenuScreen(_font, GraphicsDevice, this, _editRocketPropertiesScreen, this);

        // Initialize the map view
        _mapView = new MapView(GraphicsDevice, _pixel);
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
            if (_menuScreen.IsMenuActive)
                _menuScreen.CloseCurrentMenu();
            else
                _menuScreen.OpenMenu();
            _escapeDebounceTime = DebounceDelay; // Reset the debounce timer
        }

        // Open or close Map View 
        if (keyboardState.IsKeyDown(Keys.M) && _mDebounceTime <= 0)
        {
            if (_mapView.IsMapViewActive)
                _mapView.CloseMapView(); 
            else
                _mapView.OpenMapView(); 
            _mDebounceTime = DebounceDelay; // Reset the debounce timer
        }

        // WHen menu is open, update the menu but pause simulation by not updating rocketCurrentState
        if (_menuScreen.IsMenuActive)
            _menuScreen.Update(this, RocketCurrentState, RocketInitialPhysicsPosition,
                _rocketInitialProperties); 
        else
        {
            RocketCurrentState.Update(gameTime, keyboardState, _planet);
            _orbitElements =
                Physics.ComputeOrbit(RocketCurrentState.Position, RocketCurrentState.Velocity, _planet.Mass);
        }
        
        //necessary base class.Update from MonoGame framework
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
                _mapView.Draw(_spriteBatch, RocketCurrentState, _planet, _earthMapViewTexture, _font, _orbitElements);
            }
            else
            {
                var rocketWindowPosition = new Vector2((float)_graphics.PreferredBackBufferWidth / 2,
                    (float)_graphics.PreferredBackBufferHeight / 2);

                var distanceToSurface = Vector2.Distance(RocketCurrentState.Position, _planet.Center) - _planet.Radius;

                // Draw the planet surface and atmosphere
                Planet.Draw(_spriteBatch, GraphicsDevice, distanceToSurface, _graphics.PreferredBackBufferWidth,
                    _graphics.PreferredBackBufferHeight, RocketCurrentState, _earthSurfaceTexture);

                // Draw the rocket
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && RocketCurrentState.Fuel > 0)
                    //if space key is pressed draw rocket, else draw rocket without fire
                    RocketCurrentState.Draw(_spriteBatch, _rocketTexture, rocketWindowPosition);
                else
                    RocketCurrentState.Draw(_spriteBatch, _rocketTextureNoFire, rocketWindowPosition);
            }


            // Display Text Values
            if (_font != null)
            {
                DisplayText.DisplayTextOnScreen(_spriteBatch, _font, RocketCurrentState, _planet, _orbitElements);
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    public void ResetRocket(RocketInitialProperties rocketInitialProperties)
    {
        RocketCurrentState = new RocketCurrentState(RocketInitialPhysicsPosition, rocketInitialProperties);
    }
}