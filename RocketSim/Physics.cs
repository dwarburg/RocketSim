using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class Physics
{
    public static float Tolerance = 1e-3f; // Tolerance for floating point comparisons
    private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2

    public static Vector2 ApplyGravity(Vector2 planetCenter, float planetMass, Vector2 position, 
        Vector2 acceleration, float rocketMass)
    {
        var directionToPlanet = planetCenter - position;
        var distanceToPlanetCenter = directionToPlanet.Length();
        //get direction to planet as unit vector
        directionToPlanet.Normalize();
        var gravityForce = (GravityConstant * planetMass * rocketMass) / (distanceToPlanetCenter * distanceToPlanetCenter);
        acceleration += directionToPlanet * gravityForce;
        return acceleration;
    }

    public static bool NearlyEqual(float a, float b)
    {
        return Math.Abs(a - b) < Tolerance;
    }


    public static bool IsOnGround(Planet planet, Vector2 position)
    {
        //uses distance from center instead of y =0 to check for ground collision so origin can be changed to planet center for other planets
        var distanceFromCenter = Vector2.Distance(position, planet.Center);
        return NearlyEqual(distanceFromCenter, planet.Radius);
    }

    public static bool IsUnderGround(Planet planet, Vector2 position)
    {
        //uses distance from center instead of y =0 to check for ground collision so origin can be changed to planet center for other planets
        var distanceFromCenter = Vector2.Distance(position, planet.Center);
        return distanceFromCenter < planet.Radius;
    }

    public static List<Vector2> HandleGroundCollision(Vector2 position, Vector2 initialPosition)
    {
        position = new Vector2(position.X, initialPosition.Y);
        Vector2 velocity = Vector2.Zero;
        Vector2 acceleration = Vector2.Zero;
        return new List<Vector2> { position, velocity, acceleration };
    }


    public static Vector2 HandleUnderGround(Vector2 position, Vector2 initialPosition)
    {
        // move from below ground to on ground due to rounding and floating point errors
        // uses rocket starting Y position for ground level so that origin can be changed to planet center for other planets
        position = new Vector2(position.X, initialPosition.Y);
        return position;
    }
}

