using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class RocketCurrentState(Vector2 initialPosition, RocketInitialProperties initialProperties)
{
    public Vector2 Position { get; private set; } = initialPosition;
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 Acceleration { get; private set; } = Vector2.Zero;
    public float Rotation { get; private set; }
    public float Fuel { get; private set; } = initialProperties.MaxFuel;
    public float Tolerance = 1e-3f; // Tolerance for floating point comparisons

    private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2

    public void Update(GameTime gameTime, Vector2 planetCenter,  float planetMass,
        KeyboardState keyboardState, Planet planet, float rocketHeight)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Handle rotation
        if (keyboardState.IsKeyDown(Keys.Left))
            Rotation -= MathHelper.ToRadians(90f) * dt;
        if (keyboardState.IsKeyDown(Keys.Right))
            Rotation += MathHelper.ToRadians(90f) * dt;

        // Ground collision logic
        if (IsOnGround(planet) || IsUnderGround(planet))
        {
            HandleGroundCollision();
        }
        else
        {
            // Calculate gravity
            ApplyGravity(planetCenter, planetMass);
            var directionToPlanet = planetCenter - Position;
            var distance = directionToPlanet.Length();
            directionToPlanet.Normalize();
            var gravityForce = (GravityConstant * planetMass * initialProperties.RocketMass) / (distance * distance);
            Acceleration = directionToPlanet * gravityForce;
        }
        
        
        // Handle thrust
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
    
    public void ApplyGravity(Vector2 planetCenter, float planetMass)
    {
        // Calculate the gravitational force
        var directionToPlanet = planetCenter - Position;
        var distance = directionToPlanet.Length();
        directionToPlanet.Normalize();
        var gravityForce = (GravityConstant * planetMass * initialProperties.RocketMass) / (distance * distance);
        Acceleration = directionToPlanet * gravityForce;
    }

    public bool NearlyEqual(float a, float b)
    {
        return Math.Abs(a - b) < Tolerance;
    }


    public bool IsOnGround(Planet planet)
    {
        //uses distance from center instead of y =0 to check for ground collision so origin can be changed to planet center for other planets
        var distanceFromCenter = Vector2.Distance(Position, planet.Center);
        return NearlyEqual(distanceFromCenter, planet.Radius);
    }

    public bool IsUnderGround(Planet planet)
    {
        //uses distance from center instead of y =0 to check for ground collision so origin can be changed to planet center for other planets
        var distanceFromCenter = Vector2.Distance(Position, planet.Center);
        return distanceFromCenter < planet.Radius;
    }

    public void HandleGroundCollision()
    {
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        Position = new Vector2(Position.X, initialPosition.Y);
    }

    public void HandleUnderGround()
    {
        // move from below ground to on ground due to rounding and floating point errors
        // uses rocket starting Y position for ground level so that origin can be changed to planet center for other planets
        Position = new Vector2(Position.X, initialPosition.Y);
    }

    public void ResetToInitialPosition(Vector2 initialPosition)
    {
        Position = initialPosition;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        Rotation = 0f;
        Fuel = initialProperties.MaxFuel; // Reset fuel to initial value
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D rocketTexture, Vector2 rocketWindowPosition){

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