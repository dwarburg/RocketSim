using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//TO DO - Change Ground surface to use a circle based on the planet radius instead of 50 pixels from the bottom of the screen
//TO DO - make frame stay centered on rocket
//TO DO - separate fuelCurrent from fuelMax, rename rocketMass to rocketDryMass and incorporate mass of fuel into new variable rocketMassCurrent
//TO DO - add menu screen where rocket properties can be changed

namespace RocketSim;

public class RocketSimGame : Game
{
    private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2
    private readonly GraphicsDeviceManager _graphics;

    private Planet _planet;
    private RocketCurrentState _rocketCurrentState;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _groundTexture;
    private Texture2D _rocketTexture;

    public RocketSimGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.IsFullScreen = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        // Initialize the planet
        const float earthMass = 5.972e24f; // in kg
        const float earthRadius = 6371000; // in meters
        var planetCenter = new Vector2(400, -1 * earthRadius);
        _planet = new Planet(earthMass, earthRadius, planetCenter, _graphics.PreferredBackBufferHeight, 50);

        // Initialize the rocket
        var initialRocketPosition = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, _planet.GroundY);
        var rocketProperties = new RocketInitialProperties(
            20000f,
            100f,
            20f,
            1000f
        );
        _rocketCurrentState = new RocketCurrentState(initialRocketPosition, rocketProperties);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rocketTexture = Content.Load<Texture2D>("rocket");
        _groundTexture = new Texture2D(GraphicsDevice, 1, 1);
        _groundTexture.SetData(new[] { Color.Green });

        try
        {
            _font = Content.Load<SpriteFont>("DefaultFont");
        }
        catch
        {
            _font = null;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        _rocketCurrentState.Update(gameTime, _planet.Center, GravityConstant, _planet.Mass, keyboardState);

        // Check for ground collision using the Planet class
        if (_planet.IsRocketOnGround(_rocketCurrentState.Position, _rocketTexture?.Height ?? 64))
        {
            _rocketCurrentState.HandleGroundCollision(_planet.GroundY, _rocketTexture?.Height ?? 64);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        _spriteBatch.Draw(
            _rocketTexture,
            _rocketCurrentState.Position,
            null,
            Color.White,
            _rocketCurrentState.Rotation,
            new Vector2(_rocketTexture.Width / 2f, _rocketTexture.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );

        _spriteBatch.Draw(_groundTexture, new Rectangle(0, (int)_planet.GroundY, _graphics.PreferredBackBufferWidth, 5),
            Color.Green);

        if (_font != null)
            _spriteBatch.DrawString(_font, $"Fuel: {_rocketCurrentState.Fuel:F1}", new Vector2(10, 10), Color.White);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
