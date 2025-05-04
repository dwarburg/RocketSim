namespace RocketSim;

public class RocketProperties(float thrustPower, float fuel, float fuelBurnRate, float rocketMass)
{
    public float ThrustPower { get; set; } = thrustPower; // in Newtons
    public float Fuel { get; set; } = fuel; // in units
    public float FuelBurnRate { get; set; } = fuelBurnRate; // in units per second
    public float RocketMass { get; set; } = rocketMass; // in kilograms
}