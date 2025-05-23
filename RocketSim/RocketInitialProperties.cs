namespace RocketSim;

public class RocketInitialProperties(
    float thrustPower = 400000f, //newtons //unrealistically high to test orbit
    float maxFuel = 1000f, //kg
    float fuelBurnRate = 20f, //kg/s
    float rocketDryMass = 1000f) //kg
{
    public float ThrustPower { get; private set; } = thrustPower;

    public float MaxFuel { get; private set; } = maxFuel;

    public float FuelBurnRate { get; private set; } = fuelBurnRate;

    public float RocketDryMass { get; private set; } = rocketDryMass;

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

    public void SetRocketDryMass(float value)
    {
        RocketDryMass = value;
    }
}