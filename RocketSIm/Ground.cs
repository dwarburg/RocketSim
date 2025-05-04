namespace RocketSim;

public class Ground(float screenHeight, float buffer)
{
    public float Y { get; } = screenHeight - buffer;
}