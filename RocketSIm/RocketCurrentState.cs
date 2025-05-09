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
    public float Fuel => initialProperties.Fuel; 

    private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2

    public void Update(GameTime gameTime, Vector2 planetCenter,  float earthMass,
        KeyboardState keyboardState, Planet planet, float rocketHeight)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Handle rotation
        if (keyboardState.IsKeyDown(Keys.Left))
            Rotation -= MathHelper.ToRadians(90f) * dt;
        if (keyboardState.IsKeyDown(Keys.Right))
            Rotation += MathHelper.ToRadians(90f) * dt;

        // Ground collision logic
        if (IsOnGround(planet, rocketHeight))
        {
            HandleGroundCollision(0f, rocketHeight);
        }
        else
        {
            // Calculate gravity
            var directionToPlanet = planetCenter - Position;
            var distance = directionToPlanet.Length();
            directionToPlanet.Normalize();
            var gravityForce = (GravityConstant * earthMass * initialProperties.RocketMass) / (distance * distance);
            //var gravityForce = 0f; 
            Acceleration = directionToPlanet * gravityForce;
        }
        
        // Handle thrust
        if (keyboardState.IsKeyDown(Keys.Space) && initialProperties.Fuel > 0)
        {
            var thrust = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation)) *
                         initialProperties.ThrustPower;
            Acceleration += thrust;
            initialProperties.Fuel -= initialProperties.FuelBurnRate * dt;
            if (initialProperties.Fuel < 0) initialProperties.Fuel = 0;
        }

        // Update velocity and position
        Velocity += Acceleration * dt;
        Position += Velocity * dt;
    }

    public bool IsOnGround(Planet planet, float rocketHeight)
    {
        var distanceFromCenter = Vector2.Distance(Position, planet.Center);
        return distanceFromCenter <= planet.Radius + (rocketHeight / 2f);
    }

    public void HandleGroundCollision(float groundY, float rocketHeight)
    {
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
    }
}