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
        if (IsOnOrUnderGround(planet))
        {
            HandleGroundCollision();
            if (IsUnderGround(planet))
            {
                HandleUnderGround();
            }
        }
        else
        {
            // Calculate gravity
            var directionToPlanet = planetCenter - Position;
            var distance = directionToPlanet.Length();
            directionToPlanet.Normalize();
            var gravityForce = (GravityConstant * earthMass * initialProperties.RocketMass) / (distance * distance);
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

    public bool IsOnOrUnderGround(Planet planet)
    {
        //uses distance from center instead of y =0 to check for ground collision so origin can be changed to planet center for other planets
        var distanceFromCenter = Vector2.Distance(Position, planet.Center);
        return distanceFromCenter <= planet.Radius;
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