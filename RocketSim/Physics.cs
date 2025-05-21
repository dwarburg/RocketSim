using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace RocketSim;

public class OrbitElements
{
    public Vector2 EccentricityVector;
    public float Eccentricity;
    public float SemiMajorAxis;
    public float SemiMinorAxis;
    public Vector2 Periapsis;
};

public class Physics
{
    private static readonly float GravityConstant = 6.67430e-11f; // in m^3 kg^-1 s^-2

    private static readonly float Tolerance = 1e-3f; // Tolerance for floating point comparisons

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


    //function to determine if orbit is closed and elliptical
    public static bool OrbitIsEllipse()
    {
        return true; // Placeholder for actual calculation
    }

    //define function calculateOrbitDimensions that returns a list of 2 floats (x radius and y radius)
    public static float[] CalculateEllipticalOrbit(Vector2 position, Vector2 velocity, float mass)
    {
        // Placeholder for actual calculations
        var xRadius = 6371000f * 2f /24000; // Placeholder calculation
        var yRadius = 6371000f * 2f / 24000; // Placeholder calculation
        return [xRadius, yRadius];
    }

    public static OrbitElements ComputeOrbit(Vector2 position, Vector2 velocity, float centralMassKg)
    {
        // Compute the standard gravitational parameter μ = G * M
        float mu = GravityConstant * centralMassKg;

        float r = position.Length();        // Magnitude of position vector
        float v = velocity.Length();        // Magnitude of velocity vector
        float rv = Vector2.Dot(position, velocity); // Dot product of r and v

        // Specific angular momentum (scalar in 2D)
        float h = position.X * velocity.Y - position.Y * velocity.X;

        // Compute eccentricity vector
        Vector2 eVec = ((v * v - mu / r) * position - rv * velocity) / mu;
        float e = eVec.Length();

        // Compute semi-major axis
        float a = 1f / (2f / r - (v * v) / mu);

        // Compute semi-minor axis
        float b = a * (float)Math.Sqrt(1f - e * e);

        // Compute periapsis position vector (relative to focus at origin)
        Vector2 periapsis = Vector2.Normalize(eVec) * a;

        return new OrbitElements
        {
            Eccentricity = e,
            EccentricityVector = eVec,
            SemiMajorAxis = a,
            SemiMinorAxis = b,
            Periapsis = periapsis
        };
    }
}


