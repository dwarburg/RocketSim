namespace RocketSim.Tests;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;
using RocketSim;

public class RocketCurrentStateTests
{
    private static RocketInitialProperties GetDefaultProps() =>
        new RocketInitialProperties(thrustPower: 1000f, fuel: 100f, fuelBurnRate: 10f, rocketMass: 100f);

    private static Planet GetDefaultPlanet(Vector2? center = null, float? radius = null)
    {
        return new Planet(
            mass: Planet.DefaultMass,
            radius: radius ?? Planet.DefaultRadius,
            center: center ?? new Vector2(0, 0),
            screenHeight: 1080,
            groundBuffer: 50
        );
    }

    [Fact]
    public void Constructor_SetsInitialPosition()
    {
        var initialPosition = new Vector2(10, 20);
        var rocket = new RocketCurrentState(initialPosition, GetDefaultProps());

        Assert.Equal(initialPosition, rocket.Position);
    }

    [Fact]
    public void ResetToInitialPosition_ResetsState()
    {
        var initialPosition = new Vector2(10, 20);
        var rocket = new RocketCurrentState(initialPosition, GetDefaultProps());

        // Simulate movement
        rocket.UpdateInitialProperties(p => p.maxFuel = 50f);
        rocket.ResetToInitialPosition(new Vector2(5, 5));

        Assert.Equal(new Vector2(5, 5), rocket.Position);
        Assert.Equal(Vector2.Zero, rocket.Velocity);
        Assert.Equal(Vector2.Zero, rocket.Acceleration);
        Assert.Equal(0f, rocket.Rotation);
    }

    [Fact]
    public void UpdateInitialProperties_UpdatesProperties()
    {
        var rocket = new RocketCurrentState(Vector2.Zero, GetDefaultProps());
        rocket.UpdateInitialProperties(p => p.ThrustPower = 1234f);

        var props = rocket.GetInitialProperties();
        Assert.Equal(1234f, props.ThrustPower);
    }

    [Fact]
    public void GetInitialProperties_ReturnsCopy()
    {
        var rocket = new RocketCurrentState(Vector2.Zero, GetDefaultProps());
        var props = rocket.GetInitialProperties();
        props.ThrustPower = 9999f;

        // Should not affect the internal state
        Assert.NotEqual(9999f, rocket.GetInitialProperties().ThrustPower);
    }

    [Fact]
    public void IsOnOrUnderGround_ReturnsTrue_WhenOnGround()
    {
        var planet = GetDefaultPlanet(center: new Vector2(0, 0), radius: 10f);
        var rocket = new RocketCurrentState(new Vector2(0, 10), GetDefaultProps());

        Assert.True(rocket.IsOnOrUnderGround(planet));
    }

    [Fact]
    public void IsOnOrUnderGround_ReturnsFalse_WhenAboveGround()
    {
        var planet = GetDefaultPlanet(center: new Vector2(0, 0), radius: 10f);
        var rocket = new RocketCurrentState(new Vector2(0, 20), GetDefaultProps());

        Assert.False(rocket.IsOnOrUnderGround(planet));
    }

    [Fact]
    public void IsUnderGround_ReturnsTrue_WhenBelowGround()
    {
        var planet = GetDefaultPlanet(center: new Vector2(0, 0), radius: 10f);
        var rocket = new RocketCurrentState(new Vector2(0, 5), GetDefaultProps());

        Assert.True(rocket.IsUnderGround(planet));
    }

    [Fact]
    public void HandleGroundCollision_ResetsVelocityAndAcceleration()
    {
        var rocket = new RocketCurrentState(Vector2.Zero, GetDefaultProps());
        rocket.UpdateInitialProperties(p => { p.maxFuel = 50f; });
        rocket.ResetToInitialPosition(Vector2.Zero);

        // Simulate movement
        rocket.UpdateInitialProperties(p => { p.maxFuel = 50f; });
        rocket.Update(
            new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1)),
            Vector2.Zero, 1, new KeyboardState(), GetDefaultPlanet(), 10f
        );

        rocket.HandleGroundCollision();

        Assert.Equal(Vector2.Zero, rocket.Velocity);
        Assert.Equal(Vector2.Zero, rocket.Acceleration);
    }

    [Fact]
    public void HandleUnderGround_ResetsYToInitial()
    {
        var initialPosition = new Vector2(10, 20);
        var rocket = new RocketCurrentState(initialPosition, GetDefaultProps());
        rocket.UpdateInitialProperties(p => { p.maxFuel = 50f; });

        // Move rocket below ground
        rocket.ResetToInitialPosition(new Vector2(10, 5));
        rocket.HandleUnderGround();

        Assert.Equal(initialPosition.Y, rocket.Position.Y);
    }

    [Fact]
    public void Update_HandlesRotation()
    {
        var rocket = new RocketCurrentState(Vector2.Zero, GetDefaultProps());
        var leftState = new KeyboardState(Keys.Left);
        var rightState = new KeyboardState(Keys.Right);

        var planet = GetDefaultPlanet();
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));

        rocket.Update(gameTime, planet.Center, planet.Mass, leftState, planet, 10f);
        Assert.True(rocket.Rotation < 0);

        rocket.ResetToInitialPosition(Vector2.Zero);
        rocket.Update(gameTime, planet.Center, planet.Mass, rightState, planet, 10f);
        Assert.True(rocket.Rotation > 0);
    }

    [Fact]
    public void Update_HandlesThrustAndFuel()
    {
        var props = GetDefaultProps();
        var rocket = new RocketCurrentState(Vector2.Zero, props);

        var state = new KeyboardState(Keys.Space);
        var planet = GetDefaultPlanet();
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));

        float initialFuel = props.maxFuel;
        rocket.Update(gameTime, planet.Center, planet.Mass, state, planet, 10f);

        Assert.True(rocket.Acceleration.Length() > 0);
        Assert.True(props.maxFuel < initialFuel);
    }
}
