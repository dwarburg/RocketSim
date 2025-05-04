using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class RocketCurrentState(Vector2 initialPosition, RocketInitialProperties initialProperties)
{
    public Vector2 Position { get; private set; } = initialPosition;
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 Acceleration { get; private set; } = Vector2.Zero;
    public float Rotation { get; private set; }
    public float Fuel => initialProperties.Fuel; // Expose Fuel as a read-only property

    public void Update(GameTime gameTime, Vector2 planetCenter, float gravityConstant, float earthMass,
        KeyboardState keyboardState)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Handle rotation
        if (keyboardState.IsKeyDown(Keys.Left))
            Rotation -= MathHelper.ToRadians(90f) * dt;
        if (keyboardState.IsKeyDown(Keys.Right))
            Rotation += MathHelper.ToRadians(90f) * dt;

        // Calculate gravity
        var directionToPlanet = planetCenter - Position;
        var distance = directionToPlanet.Length();
        directionToPlanet.Normalize();
        var gravityForce = -gravityConstant * earthMass * initialProperties.RocketMass / (distance * distance);
        Acceleration = directionToPlanet * gravityForce;

        // Handle thrust
        if (keyboardState.IsKeyDown(Keys.Space) && initialProperties.Fuel > 0)
        {
            var thrust = new Vector2((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation)) *
                         initialProperties.ThrustPower;
            Acceleration += thrust;
            initialProperties.Fuel -= initialProperties.FuelBurnRate * dt;
            if (initialProperties.Fuel < 0) initialProperties.Fuel = 0;
        }

        // Update velocity and position
        Velocity += Acceleration * dt;
        Position += Velocity * dt;
    }

    public void HandleGroundCollision(float groundY, float rocketHeight)
    {
        if (!(Position.Y + rocketHeight / 2f >= groundY)) return;
        Position = new Vector2(Position.X, groundY - rocketHeight / 2f);
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
    }
}