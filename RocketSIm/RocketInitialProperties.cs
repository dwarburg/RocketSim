namespace RocketSim;

public class RocketInitialProperties(
    float thrustPower = 20000f,
    float maxFuel = 100f,
    float fuelBurnRate = 20f,
    float rocketMass = 1000f)
{
    public float ThrustPower { get; private set; } = thrustPower;

    public float MaxFuel { get; private set; } = maxFuel;

    public float FuelBurnRate { get; private set; } = fuelBurnRate;

    public float RocketMass { get; private set; } = rocketMass;

    // Controlled modification methods
    public void SetThrustPower(float value)
    {
        ThrustPower = value;
    }

    public void SetMaxFuel(float value)
    {
        MaxFuel = value;
    }

    public void SetFuelBurnRate(float value)
    {
        FuelBurnRate = value;
    }

    public void SetRocketMass(float value)
    {
        RocketMass = value;
    }
}