using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RocketSim;

public class Physics
{
    private const float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2
    public static float Tolerance = 1e-3f; // Tolerance for floating point comparisons

    public static float ForceDueToGravity(Vector2 planetCenter, float planetMass, Vector2 position,
        Vector2 acceleration, float rocketMass)
    {
        var directionToPlanet = planetCenter - position;
        var distanceToPlanetCenter = directionToPlanet.Length();
        var gravityForce = GravityConstant * planetMass * rocketMass /
                           (distanceToPlanetCenter * distanceToPlanetCenter);
        return gravityForce;
    }


    public static Vector2 AccelerationDueToGravity(Vector2 planetCenter, float planetMass, Vector2 position,
        float rocketMass, float gravityForce)
    {
        var acceleration = Vector2.Zero;
        var directionToPlanet = planetCenter - position;
        //get direction to planet as unit vector
        directionToPlanet.Normalize();
        acceleration += directionToPlanet * gravityForce;
        return acceleration;
    }

    public static bool NearlyEqual(float a, float b)
    {
        return Math.Abs(a - b) < Tolerance;
    }

}