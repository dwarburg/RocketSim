using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace RocketSim
{

    public class Rocket
    {
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public Vector2 Acceleration { get; private set; }
        public float Rotation { get; private set; }

        private RocketProperties _properties;

        public Rocket(Vector2 initialPosition, RocketProperties properties)
        {
            Position = initialPosition;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Rotation = 0f;
            _properties = properties;
        }
        public float Fuel => _properties.Fuel; // Expose Fuel as a read-only property

        public void Update(GameTime gameTime, Vector2 planetCenter, float gravityConstant, float earthMass,
            KeyboardState keyboardState)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle rotation
            if (keyboardState.IsKeyDown(Keys.Left))
                Rotation -= MathHelper.ToRadians(90f) * dt;
            if (keyboardState.IsKeyDown(Keys.Right))
                Rotation += MathHelper.ToRadians(90f) * dt;

            // Calculate gravity
            Vector2 directionToPlanet = planetCenter - Position;
            float distance = directionToPlanet.Length();
            directionToPlanet.Normalize();
            float gravityForce = -gravityConstant * earthMass * _properties.RocketMass / (distance * distance);
            Acceleration = directionToPlanet * gravityForce;

            // Handle thrust
            if (keyboardState.IsKeyDown(Keys.Space) && _properties.Fuel > 0)
            {
                Vector2 thrust = new Vector2((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation)) *
                                 _properties.ThrustPower;
                Acceleration += thrust;
                _properties.Fuel -= _properties.FuelBurnRate * dt;
                if (_properties.Fuel < 0) _properties.Fuel = 0;
            }

            // Update velocity and position
            Velocity += Acceleration * dt;
            Position += Velocity * dt;
        }

        public void HandleGroundCollision(float groundY, float rocketHeight)
        {
            if (Position.Y + rocketHeight / 2f >= groundY)
            {
                Position = new Vector2(Position.X, groundY - rocketHeight / 2f);
                Velocity = Vector2.Zero;
                Acceleration = Vector2.Zero;
            }
        }
    }
}