namespace RocketSim;

public class RocketInitialProperties
{
    public float ThrustPower { get; set; }
    public float Fuel { get; set; }
    public float FuelBurnRate { get; set; }
    public float RocketMass { get; set; }

    // Constructor with default values
    public RocketInitialProperties(
        float thrustPower = 20000f,
        float fuel = 100f,
        float fuelBurnRate = 20f,
        float rocketMass = 1000f)
    {
        ThrustPower = thrustPower;
        Fuel = fuel;
        FuelBurnRate = fuelBurnRate;
        RocketMass = rocketMass;
    }
}
