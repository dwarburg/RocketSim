using System;
using Microsoft.Xna.Framework;

namespace RocketSim;

public class OrbitElements
{
    public Vector2 Apoapsis;
    public float Eccentricity;
    public Vector2 EccentricityVector;
    public Vector2 Periapsis;
    public float SemiMajorAxis;
    public float SemiMinorAxis;
}

public class Physics
{
    private const float GravityConstant = 6.67430e-11f; // m^3 kg^-1 s^-2
    private const float Tolerance = 1e-3f; // floating point tolerance

    public static Vector2 AccelerationDueToGravity(Vector2 planetCenter, float planetMass, Vector2 position)
    {
        var directionToPlanet = planetCenter - position;
        var distanceToPlanetCenter = directionToPlanet.Length();
        directionToPlanet.Normalize();
        // Newton's law: a = G * M / r^2
        var accelerationMagnitude = GravityConstant * planetMass / (distanceToPlanetCenter * distanceToPlanetCenter);
        return directionToPlanet * accelerationMagnitude;
    }

    private static Vector2 ComputePeriapsis(Vector2 eVec, float a, float e)
    {
        if (e < 1e-6f) return Vector2.Zero; // Circular orbit
        if (a <= 0 || float.IsNaN(e) || float.IsNaN(a)) return Vector2.Zero;

        var direction = Vector2.Normalize(eVec);
        return direction * (a * (1 - e));
    }

    private static Vector2 ComputeApoapsis(Vector2 eVec, float a, float e)
    {
        if (e < 1e-6f) return Vector2.Zero; // Circular orbit
        if (a <= 0 || float.IsNaN(e) || float.IsNaN(a)) return Vector2.Zero;

        var direction = Vector2.Normalize(eVec);
        return direction * (a * (1 + e));
    }

    public static bool OrbitIsEllipse(float eccentricity)
    {
        return eccentricity < 1.0f;
    }

    public static OrbitElements ComputeOrbit(Vector2 position, Vector2 velocity, float centralMassKg)
    {
        var mu = GravityConstant * centralMassKg;

        var r = position.Length();
        var v = velocity.Length();
        var rv = Vector2.Dot(position, velocity);

        if (v < Tolerance)
            return new OrbitElements
            {
                Eccentricity = 1f,
                EccentricityVector = -Vector2.Normalize(position),
                SemiMajorAxis = float.NaN,
                SemiMinorAxis = float.NaN,
                Periapsis = Vector2.Zero,
                Apoapsis = Vector2.Zero
            };

        var eVector = ((v * v - mu / r) * position - rv * velocity) / mu;
        var eMagnitude = eVector.Length();

        var semiMajorAxis = 1f / (2f / r - v * v / mu);
        var semiMinorAxis = semiMajorAxis * (float)Math.Sqrt(1f - eMagnitude * eMagnitude);
        var periapsis = ComputePeriapsis(eVector, semiMajorAxis, eMagnitude);
        var apoapsis = ComputeApoapsis(eVector, semiMajorAxis, eMagnitude);

        return new OrbitElements
        {
            Eccentricity = eMagnitude,
            EccentricityVector = eVector,
            SemiMajorAxis = semiMajorAxis,
            SemiMinorAxis = semiMinorAxis,
            Periapsis = periapsis,
            Apoapsis = apoapsis
        };
    }
}