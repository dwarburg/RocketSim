using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketSim;

public class Planet(float mass, float radius, Vector2 center, float screenHeight, float groundBuffer)
{
    public const float DefaultMass = 5.972e24f; // in kg
    public const float DefaultRadius = 6371000f; // in meters

    public float Mass { get; } = mass;
    public float Radius { get; } = radius;
    public Vector2 Center { get; } = center;
    
    private Texture2D _planetTexture;

}
