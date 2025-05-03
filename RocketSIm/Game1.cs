using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RocketSim
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D rocketTexture;
        Texture2D groundTexture;
        SpriteFont font;

        Vector2 rocketPosition;
        Vector2 rocketVelocity;
        Vector2 rocketAcceleration;

        float gravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2
        Vector2 planetCenter;

        private RocketProperties _rocketProperties;

        //float thrustPower = 20000f; //in Newtons (which are kg*m/s^2)
        float rocketRotation = 0f;
        float rotationSpeed = MathHelper.ToRadians(90f);

        //float fuel = 100f;
        //float fuelBurnRate = 20f;

        float groundY;

        //float rocketMass = 1000f; // in kg
        float earthMass = 5.972e24f; // in kg

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            // Set full screen mode
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

            int groundBuffer = 50; // 50 pixels above the bottom of the screen
            groundY = _graphics.PreferredBackBufferHeight - groundBuffer;

            // Set the initial state vectors of the rocket
            int middleOfScreen = _graphics.PreferredBackBufferWidth / 2;
            rocketPosition = new Vector2(middleOfScreen, groundY);
            rocketVelocity = new Vector2(0, 0);
            rocketAcceleration = Vector2.Zero;

            // Initialize RocketProperties
            _rocketProperties = new RocketProperties(
                thrustPower: 20000f, // in Newtons
                fuel: 100f, // in units
                fuelBurnRate: 20f, // in units per second
                rocketMass: 1000f // in kilograms
            );

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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.Left))
                rocketRotation -= rotationSpeed * dt;
            if (keyboardState.IsKeyDown(Keys.Right))
                rocketRotation += rotationSpeed * dt;

            Vector2 directionToPlanet = planetCenter - rocketPosition;
            float distance = directionToPlanet.Length();
            directionToPlanet.Normalize();
            float gravityForce = -gravityConstant * earthMass * _rocketProperties.RocketMass / (distance * distance);

            rocketAcceleration = directionToPlanet * gravityForce;

            if (keyboardState.IsKeyDown(Keys.Space) && _rocketProperties.Fuel > 0)
            {
                Vector2 thrust = new Vector2((float)Math.Sin(rocketRotation), -(float)Math.Cos(rocketRotation)) * _rocketProperties.ThrustPower;
                rocketAcceleration += thrust;
                _rocketProperties.Fuel -= _rocketProperties.FuelBurnRate * dt;
                if (_rocketProperties.Fuel < 0) _rocketProperties.Fuel = 0;
            }

            rocketVelocity += rocketAcceleration * dt;
            rocketPosition += rocketVelocity * dt;

            float rocketHeight = rocketTexture?.Height ?? 64;
            
            //set velocity to zero when rocket hits the ground
            if (rocketPosition.Y + rocketHeight / 2f >= groundY)
            {
                rocketPosition.Y = groundY - rocketHeight / 2f;
                rocketVelocity = Vector2.Zero;
                rocketAcceleration = Vector2.Zero;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                rocketTexture,
                rocketPosition,
                null,
                Color.White,
                rocketRotation,
                new Vector2(rocketTexture.Width / 2f, rocketTexture.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);
            _spriteBatch.Draw(groundTexture, new Rectangle(0, (int)groundY, _graphics.PreferredBackBufferWidth, 5), Color.Green);
            if (font != null)
                _spriteBatch.DrawString(font, $"Fuel: {_rocketProperties.Fuel:F1}", new Vector2(10, 10), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
