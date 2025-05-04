using Microsoft.Xna.Framework;

public class Ground
{
    public float Y { get; private set; }

    public Ground(float screenHeight, float buffer)
    {
        Y = screenHeight - buffer;
    }

    public bool IsRocketOnGround(Vector2 rocketPosition, float rocketHeight)
    {
        return rocketPosition.Y + rocketHeight / 2f >= Y;
    }
}
