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

    private static Vector2 ComputePeriapsis(Vector2 eVec, float a, float e)
    {
        if (e < 1e-6f) return Vector2.Zero; // Circular
        if (a <= 0 || float.IsNaN(e) || float.IsNaN(a)) return Vector2.Zero;

        Vector2 direction = Vector2.Normalize(eVec);
        return direction * (a * (1 - e));
    }

    //function to determine if orbit is closed and elliptical
    public static bool OrbitIsEllipse(float eccentricity)
    {
        return eccentricity < 1.0f;
    }

    public static string ClassifyOrbit(float eccentricity)
    {
        if (eccentricity < 1e-6f) return "Circular";
        if (eccentricity < 1f) return "Elliptical";
        if (Math.Abs(eccentricity - 1f) < 1e-3f) return "Parabolic";
        return "Hyperbolic";
    }

    public static OrbitElements ComputeOrbit(Vector2 position, Vector2 velocity, float centralMassKg)
    {
        


        // Compute the standard gravitational parameter μ = G * M
        float mu = GravityConstant * centralMassKg;

        float r = position.Length();        // Magnitude of position vector
        float v = velocity.Length();        // Magnitude of velocity vector
        float rv = Vector2.Dot(position, velocity); // Dot product of r and v

        if (v < Tolerance)
        {
            return new OrbitElements
            {
                Eccentricity = 1f,
                EccentricityVector = -Vector2.Normalize(position),
                SemiMajorAxis = float.NaN,
                SemiMinorAxis = float.NaN,
                Periapsis = Vector2.Zero // Technically at the focus (center)
            };
        }

        // Compute eccentricity vector
        Vector2 eVector = ((v * v - mu / r) * position - rv * velocity) / mu;
        float eMagnitude = eVector.Length();

        // Compute semi-major axis
        float semiMajorAxis = 1f / (2f / r - (v * v) / mu);

        // Compute semi-minor axis
        float semiMinorAxis = semiMajorAxis * (float)Math.Sqrt(1f - eMagnitude * eMagnitude);

        // Compute periapsis position vector (relative to focus at origin)
        Vector2 periapsis = ComputePeriapsis(eVector, semiMajorAxis, eMagnitude);

        return new OrbitElements
        {
            Eccentricity = eMagnitude,
            EccentricityVector = eVector,
            SemiMajorAxis = semiMajorAxis,
            SemiMinorAxis = semiMinorAxis,
            Periapsis = periapsis
        };
    }
}


