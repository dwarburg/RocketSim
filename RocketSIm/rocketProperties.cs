namespace RocketSim
{
    public class RocketProperties
    {
        public float ThrustPower { get; set; } // in Newtons
        public float Fuel { get; set; } // in units
        public float FuelBurnRate { get; set; } // in units per second
        public float RocketMass { get; set; } // in kilograms

        public RocketProperties(float thrustPower, float fuel, float fuelBurnRate, float rocketMass)
        {
            ThrustPower = thrustPower;
            Fuel = fuel;
            FuelBurnRate = fuelBurnRate;
            RocketMass = rocketMass;
        }
    }
}
