using System;

namespace RocketSim;

public class RocketInitialProperties
{
    private float _thrustPower;
    private float _maxFuel;
    private float _fuelBurnRate;
    private float _rocketMass;

    public float ThrustPower => _thrustPower;
    public float MaxFuel => _maxFuel;
    public float FuelBurnRate => _fuelBurnRate;
    public float RocketMass => _rocketMass;

    public RocketInitialProperties(
        float thrustPower = 20000f,
        float maxFuel = 100f,
        float fuelBurnRate = 20f,
        float rocketMass = 1000f)
    {
        _thrustPower = thrustPower;
        _maxFuel = maxFuel;
        _fuelBurnRate = fuelBurnRate;
        _rocketMass = rocketMass;
    }

    // Controlled modification methods
    public void SetThrustPower(float value) => _thrustPower = value;
    public void SetMaxFuel(float value) => _maxFuel = value;
    public void SetFuelBurnRate(float value) => _fuelBurnRate = value;
    public void SetRocketMass(float value) => _rocketMass = value;
}
