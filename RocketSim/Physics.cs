using System;
using Microsoft.Xna.Framework;

namespace RocketSim
{
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
        private static readonly float GravityConstant = 6.67430e-11f; // m^3 kg^-1 s^-2
        private static readonly float Tolerance = 1e-3f; // floating point tolerance

        public static Vector2 AccelerationDueToGravity(Vector2 planetCenter, float planetMass, Vector2 position)
        {
            var directionToPlanet = planetCenter - position;
            var distanceToPlanetCenter = directionToPlanet.Length();
            directionToPlanet.Normalize();
            // Newton's law: a = G * M / r^2
            float accelerationMagnitude = GravityConstant * planetMass / (distanceToPlanetCenter * distanceToPlanetCenter);
            return directionToPlanet * accelerationMagnitude;
        }

        private static Vector2 ComputePeriapsis(Vector2 eVec, float a, float e)
        {
            if (e < 1e-6f) return Vector2.Zero; // Circular orbit
            if (a <= 0 || float.IsNaN(e) || float.IsNaN(a)) return Vector2.Zero;

            Vector2 direction = Vector2.Normalize(eVec);
            return direction * (a * (1 - e));
        }

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
            float mu = GravityConstant * centralMassKg;

            float r = position.Length();
            float v = velocity.Length();
            float rv = Vector2.Dot(position, velocity);

            if (v < Tolerance)
            {
                return new OrbitElements
                {
                    Eccentricity = 1f,
                    EccentricityVector = -Vector2.Normalize(position),
                    SemiMajorAxis = float.NaN,
                    SemiMinorAxis = float.NaN,
                    Periapsis = Vector2.Zero
                };
            }

            Vector2 eVector = ((v * v - mu / r) * position - rv * velocity) / mu;
            float eMagnitude = eVector.Length();

            float semiMajorAxis = 1f / (2f / r - (v * v) / mu);
            float semiMinorAxis = semiMajorAxis * (float)Math.Sqrt(1f - eMagnitude * eMagnitude);
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
}
