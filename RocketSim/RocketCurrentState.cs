using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class RocketCurrentState(Vector2 initialPosition, RocketInitialProperties initialProperties)
{
    public Vector2 Position { get; private set; } = new(initialPosition.X, initialPosition.Y);
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 Acceleration { get; private set; } = Vector2.Zero;
    public float Rotation { get; private set; }
    public float Fuel { get; private set; } = initialProperties.MaxFuel;

    public float RocketTotalMass => initialProperties.RocketDryMass + Fuel;

    public void Update(GameTime gameTime, Vector2 planetCenter, float planetMass,
        KeyboardState keyboardState, Planet planet, float rocketHeight)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Handle rotation
        if (keyboardState.IsKeyDown(Keys.Left))
            Rotation -= MathHelper.ToRadians(90f) * dt;
        if (keyboardState.IsKeyDown(Keys.Right))
            Rotation += MathHelper.ToRadians(90f) * dt;

        // write message to visual studio debugger
        //System.Diagnostics.Debug.WriteLine($"Before: Position={Position}, Velocity={Velocity}");

        // Ground collision logic based on equation of a circle of planet radius
        // Ground collision logic based on equation of a circle of planet radius
        if (Position.X * Position.X + Position.Y * Position.Y <= planet.Radius * planet.Radius)
        {
            // Calculate the normal vector from the planet center to the rocket
            var fromCenter = Vector2.Normalize(Position - planet.Center);

            // If velocity is moving into the planet, snap to surface and zero velocity
            if (Vector2.Dot(Velocity, fromCenter) < 0)
            {
                var newY = (float)Math.Sqrt(planet.Radius * planet.Radius - Position.X * Position.X);
                if (Position.Y < 0) newY = -newY;
                Position = new Vector2(Position.X, newY);
                Velocity = Vector2.Zero;
                Acceleration = Vector2.Zero;
            }
            // Otherwise, let the rocket move away from the surface
        }
        else
        {
            //Acceleration Due to Gravity
            Acceleration = Physics.AccelerationDueToGravity(planet.Center, planet.Mass, Position);
        }


        // Apply thrust whether rocket is on ground or not
        if (keyboardState.IsKeyDown(Keys.Space) && Fuel > 0)
        {
            var thrust = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation)) *
                         initialProperties.ThrustPower;
            Acceleration += thrust / RocketTotalMass;
            Fuel -= initialProperties.FuelBurnRate * dt;
            if (Fuel < 0) Fuel = 0;
        }

        // Update velocity and position
        Velocity += Acceleration * dt;
        Position += Velocity * dt;
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