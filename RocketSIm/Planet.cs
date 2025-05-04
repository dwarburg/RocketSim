using Microsoft.Xna.Framework;

namespace RocketSim;

public class Planet
{
    public float Mass { get; }
    public float Radius { get; }
    public Vector2 Center { get; }
    public float GroundY { get; }

    public Planet(float mass, float radius, Vector2 center, float screenHeight, float groundBuffer)
    {
        Mass = mass;
        Radius = radius;
        Center = center;

        // Calculate ground height based on the screen height and buffer
        GroundY = screenHeight - groundBuffer;
    }

    public bool IsRocketOnGround(Vector2 rocketPosition, float rocketHeight)
    {
        return rocketPosition.Y + rocketHeight / 2f >= GroundY;
    }
}
