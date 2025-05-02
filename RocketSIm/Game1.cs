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

        float gravityConstant = 100000f;
        Vector2 planetCenter;

        float thrustPower = 300f;
        float rocketRotation = 0f;
        float rotationSpeed = MathHelper.ToRadians(90f);

        float fuel = 100f;
        float fuelBurnRate = 20f;

        float groundY;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            // Set full screen mode
            //_graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            rocketPosition = new Vector2(400, 500);
            rocketVelocity = new Vector2(80, -100);
            rocketAcceleration = Vector2.Zero;

            planetCenter = new Vector2(400, 300);
            groundY = _graphics.PreferredBackBufferHeight - 50;

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
            float gravityForce = gravityConstant / (distance * distance);
            rocketAcceleration = directionToPlanet * gravityForce;

            if (keyboardState.IsKeyDown(Keys.Space) && fuel > 0)
            {
                Vector2 thrust = new Vector2((float)Math.Sin(rocketRotation), -(float)Math.Cos(rocketRotation)) * thrustPower;
                rocketAcceleration += thrust;
                fuel -= fuelBurnRate * dt;
                if (fuel < 0) fuel = 0;
            }

            rocketVelocity += rocketAcceleration * dt;
            rocketPosition += rocketVelocity * dt;

            float rocketHeight = rocketTexture?.Height ?? 64;
            if (rocketPosition.Y + rocketHeight / 2f >= groundY)
            {
                rocketPosition.Y = groundY - rocketHeight / 2f;
                rocketVelocity.Y = 0;
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
                _spriteBatch.DrawString(font, $"Fuel: {fuel:F1}", new Vector2(10, 10), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
