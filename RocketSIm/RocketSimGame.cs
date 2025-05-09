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
    private readonly GraphicsDeviceManager _graphics;

    private Planet _planet;
    private RocketCurrentState _rocketCurrentState;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _rocketTexture;

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
        var planetCenter = new Vector2(0, -1 * Planet.DefaultRadius);
        _planet = new Planet(Planet.DefaultMass, Planet.DefaultRadius, planetCenter, _graphics.PreferredBackBufferHeight, 50);

        // Initialize the rocket
        var rocketInitialPhysicsPosition = new Vector2(0, 0);
        var rocketProperties = new RocketInitialProperties();
        _rocketCurrentState = new RocketCurrentState(rocketInitialPhysicsPosition, rocketProperties);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rocketTexture = Content.Load<Texture2D>("rocket");

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

        // Update the rocket's state, including ground collision logic
        _rocketCurrentState.Update(gameTime, _planet.Center, _planet.Mass, keyboardState, _planet, _rocketTexture?.Height ?? 64);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var rocketWindowPosition = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2);

        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(); 
        
        // If the rocket is within 540 pixels of the surface, create the texture for the visible portion of the planet
        var distanceToSurface = Vector2.Distance(_rocketCurrentState.Position, _planet.Center) - _planet.Radius;
        if (distanceToSurface <= 540)
        {
            //draw green rectangle that takes up the bottom half of the screen
            var planetTexture = new Texture2D(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            var colorData = new Color[_graphics.PreferredBackBufferWidth * _graphics.PreferredBackBufferHeight];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = Color.Green;
            }
            planetTexture.SetData(colorData);
            var planetRectangle = new Rectangle(0, _graphics.PreferredBackBufferHeight / 2, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight / 2);
            _spriteBatch.Draw(planetTexture, planetRectangle, Color.White);


        }

        // Draw the rocket
        _spriteBatch.Draw(
            _rocketTexture,
            rocketWindowPosition,
            null,
            Color.White,
            _rocketCurrentState.Rotation,
            new Vector2(_rocketTexture.Width / 2f, _rocketTexture.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );

        

        // Display Text Values
        if (_font != null)
        {
            // Display the rocket's position
            var rocketPositionText = $"Position: X={_rocketCurrentState.Position.X:F1}, Y={_rocketCurrentState.Position.Y:F1}";
            _spriteBatch.DrawString(_font, rocketPositionText, new Vector2(10, 10), Color.White);

            // Calculate and display the distance to the planet's center
            var distanceToCenter = Vector2.Distance(_rocketCurrentState.Position, _planet.Center);
            var distanceText = $"Distance to Planet Center: {distanceToCenter:F1} meters";
            _spriteBatch.DrawString(_font, distanceText, new Vector2(10, 30), Color.White);

            // Display the fuel below the position
            _spriteBatch.DrawString(_font, $"Fuel: {_rocketCurrentState.Fuel:F1}", new Vector2(10, 50), Color.White);

            // Display the rocket's velocity
            var rocketVelocityText = $"Velocity: X={_rocketCurrentState.Velocity.X:F1}, Y={_rocketCurrentState.Velocity.Y:F1}";
            _spriteBatch.DrawString(_font, rocketVelocityText, new Vector2(10, 70), Color.White);

            // Display the rocket's acceleration
            var rocketAccelerationText = $"Acceleration: X={_rocketCurrentState.Acceleration.X:F1}, Y={_rocketCurrentState.Acceleration.Y:F1}";
            _spriteBatch.DrawString(_font, rocketAccelerationText, new Vector2(10, 90), Color.White);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
