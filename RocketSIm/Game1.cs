using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//TO DO - Change Ground surface to use a circle based on the planet radius instead of 50 pixels from the bottom of the screen
//TO DO - make frame stay centered on rocket
//TO DO - separate fuelCurrent from fuelMax, rename rocketMass to rocketDryMass and incorporate mass of fuel into new variable rocketMassCurrent
//TO DO - add menu screen where rocket properties can be changed

namespace RocketSim;

public class Game1 : Game
{
    private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2
    private const float EarthMass = 5.972e24f; // in kg
    private readonly GraphicsDeviceManager _graphics;
    private Ground _ground;

    private Rocket _rocket;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _groundTexture;

    private Vector2 _planetCenter;

    private Texture2D _rocketTexture;

    public Game1()
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

        const int earthRadius = 6371000; // in meters
        _planetCenter = new Vector2(400, -1 * earthRadius);

        _ground = new Ground(_graphics.PreferredBackBufferHeight, 50);

        var initialRocketPosition = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, _ground.Y);

        var rocketProperties = new RocketProperties(
            20000f,
            100f,
            20f,
            1000f
        );
        _rocket = new Rocket(initialRocketPosition, rocketProperties);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rocketTexture = Content.Load<Texture2D>("rocket");
        _groundTexture = new Texture2D(GraphicsDevice, 1, 1);
        _groundTexture.SetData([Color.Green]);

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

        _rocket.Update(gameTime, _planetCenter, GravityConstant, EarthMass, keyboardState);
        _rocket.HandleGroundCollision(_ground.Y, _rocketTexture?.Height ?? 64);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        _spriteBatch.Draw(
            _rocketTexture,
            _rocket.Position,
            null,
            Color.White,
            _rocket.Rotation,
            new Vector2(_rocketTexture.Width / 2f, _rocketTexture.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );

        _spriteBatch.Draw(_groundTexture, new Rectangle(0, (int)_ground.Y, _graphics.PreferredBackBufferWidth, 5),
            Color.Green);

        if (_font != null)
            _spriteBatch.DrawString(_font, $"Fuel: {_rocket.Fuel:F1}", new Vector2(10, 10), Color.White);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}