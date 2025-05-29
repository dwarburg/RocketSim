using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class RocketSimGame : Game
{
    //built in types
    private TexturesAndFonts _textures;

    //MonoGame (Microsoft.Xna.Framework) Types
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Vector2 RocketInitialPhysicsPosition;

    //My custom types
    private GameRenderer _renderer;
    private readonly RocketInitialProperties _rocketInitialProperties = new();
    private EditRocketPropertiesScreen _editRocketPropertiesScreen;
    private GameStateManager _gameStateManager;
    private MapView _mapView;
    private MenuScreen _menuScreen;
    private Planet _planet;
    public RocketCurrentState RocketCurrentState;
    private OrbitElements _orbitElements;
    private InputManager _inputManager;

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

        _planet = new Planet(Planet.DefaultMass, Planet.DefaultRadius);
        
        RocketInitialPhysicsPosition = new Vector2(0, _planet.Radius);
        RocketCurrentState = new RocketCurrentState(RocketInitialPhysicsPosition, new RocketInitialProperties());
        
        _orbitElements = new OrbitElements();

        _inputManager = new InputManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _textures = new TexturesAndFonts();

        _textures.LoadTextures(Content, GraphicsDevice);

       // Initialize the edit rocket properties screen
        _editRocketPropertiesScreen = new EditRocketPropertiesScreen(
            _textures.Font, 
            _textures.Pixel, 
            _rocketInitialProperties, 
            GraphicsDevice, 
            this
        );

        // Initialize the menu screen
        _menuScreen = new MenuScreen(
            _textures.Font, 
            GraphicsDevice,
            _editRocketPropertiesScreen, 
            this
        );

        // Initialize the map view
        _mapView = new MapView(GraphicsDevice, _textures.Pixel);

        //initialize the game state
        _gameStateManager = new GameStateManager(
            _inputManager,
            _menuScreen,
            _mapView,
            () => RocketCurrentState, //pass the current rocket state as a lambda to decouple from GameStateManager
            _planet,
            _rocketInitialProperties,
            RocketInitialPhysicsPosition
        );

        //initallize the renderer using lambdas to get current rocket state and orbit elements (decoupling)
        _renderer = new GameRenderer(
            GraphicsDevice,
            _graphics,
            _textures,
            _menuScreen,
            _mapView,
            _rocketInitialProperties,
            _planet,
            () => RocketCurrentState,
            () => _orbitElements
        );
    }

    protected override void Update(GameTime gameTime)
    {
        //my game state class
        _gameStateManager.Update(gameTime, this);
        
        //necessary base class.Update from MonoGame framework
        base.Update(gameTime);

        
    }

    protected override void Draw(GameTime gameTime)
    {
        _renderer.Draw(_spriteBatch);
        base.Draw(gameTime);
    }

    public void ResetRocket(RocketInitialProperties rocketInitialProperties)
    {
        RocketCurrentState = new RocketCurrentState(RocketInitialPhysicsPosition, rocketInitialProperties);
    }
}