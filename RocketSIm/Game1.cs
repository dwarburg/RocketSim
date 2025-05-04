using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

//TO DO - Change Ground surface to use a circle based on the planet radius instead of 50 pixels from the bottom of the screen
//TO DO - make frame stay centered on rocket
//TO DO - seperate fuelCurrent from fuelMax, rename rocketMass to rocketDryMass and incorporate mass of fuel into new variable rocketMassCurrent
//TO DO - add menu screen where rocket properties can be changed

namespace RocketSim
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D rocketTexture;
        private Texture2D groundTexture;
        private SpriteFont font;

        private Rocket _rocket;
        private Ground _ground;

        private Vector2 planetCenter;
        private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2
        private const float EarthMass = 5.972e24f; // in kg

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

            int earthRadius = 6371000; // in meters
            planetCenter = new Vector2(400, -1 * earthRadius);

            _ground = new Ground(_graphics.PreferredBackBufferHeight, 50);

            Vector2 initialRocketPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _ground.Y);
            var rocketProperties = new RocketProperties(
                thrustPower: 20000f,
                fuel: 100f,
                fuelBurnRate: 20f,
                rocketMass: 1000f
            );
            _rocket = new Rocket(initialRocketPosition, rocketProperties);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            rocketTexture = Content.Load<Texture2D>("rocket");
            groundTexture = new Texture2D(GraphicsDevice, 1, 1);
            groundTexture.SetData(new[] { Color.Green });

            try { font = Content.Load<SpriteFont>("DefaultFont"); } catch { font = null; }
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _rocket.Update(gameTime, planetCenter, GravityConstant, EarthMass, keyboardState);
            _rocket.HandleGroundCollision(_ground.Y, rocketTexture?.Height ?? 64);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                rocketTexture,
                _rocket.Position,
                null,
                Color.White,
                _rocket.Rotation,
                new Vector2(rocketTexture.Width / 2f, rocketTexture.Height / 2f),
                1f,
                SpriteEffects.None,
                0f
            );

            _spriteBatch.Draw(groundTexture, new Rectangle(0, (int)_ground.Y, _graphics.PreferredBackBufferWidth, 5), Color.Green);

            if (font != null)
                _spriteBatch.DrawString(font, $"Fuel: {_rocket.Fuel:F1}", new Vector2(10, 10), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
