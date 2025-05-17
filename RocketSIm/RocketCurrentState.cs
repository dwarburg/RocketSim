using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class RocketCurrentState(Vector2 initialPosition, RocketInitialProperties initialProperties)
{
    public Vector2 Position { get; private set; } = new (initialPosition.X, initialPosition.Y);
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 Acceleration { get; private set; } = Vector2.Zero;
    public float Rotation { get; private set; }
    public float Fuel { get; private set; } = initialProperties.MaxFuel;

    public void Update(GameTime gameTime, Vector2 planetCenter, float planetMass,
        KeyboardState keyboardState, Planet planet, float rocketHeight)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Handle rotation
        if (keyboardState.IsKeyDown(Keys.Left))
            Rotation -= MathHelper.ToRadians(90f) * dt;
        if (keyboardState.IsKeyDown(Keys.Right))
            Rotation += MathHelper.ToRadians(90f) * dt;

        // Ground collision logic
        if (Position.Y <= 0f) 
        {
            //HandleGroundCollision
            Position = new Vector2(Position.X, 0f);
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
        }
        else
        {
            //Force Due to Gravity
            var gravityForce = Physics.ForceDueToGravity(planet.Center, planet.Mass, Position,
                Acceleration, initialProperties.RocketMass);
            //Acceleration Due to Gravity
            Acceleration = Physics.AccelerationDueToGravity(planet.Center, planet.Mass, Position,
                initialProperties.RocketMass, gravityForce);
        }


        // Apply thrust whether rocket is on ground or not
        if (keyboardState.IsKeyDown(Keys.Space) && Fuel > 0)
        {
            var thrust = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation)) *
                         initialProperties.ThrustPower;
            Acceleration += thrust;
            Fuel -= initialProperties.FuelBurnRate * dt;
            if (Fuel < 0) Fuel = 0;
        }

        // Update velocity and position
        Velocity += Acceleration * dt;
        Position += Velocity * dt;
    }


    public void ResetToInitialPosition()
    {
        Position = initialPosition;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        Rotation = 0f;
        Fuel = initialProperties.MaxFuel; // Reset fuel to initial value
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D rocketTexture, Vector2 rocketWindowPosition)
    {
        // Draw the rocket
        spriteBatch.Draw(
            rocketTexture,
            rocketWindowPosition,
            null,
            Color.White,
            Rotation,
            new Vector2(rocketTexture.Width / 2f, rocketTexture.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );
    }
}