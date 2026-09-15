namespace Services.Models
{
    public sealed record SteamController
    {
        public float BatteryPercentage { get; set; } = -1;
        public SteamControllerPowerState Powerstate { get; set; }
        public bool IsCharging
        {
            get
            {
                return this.Powerstate == SteamControllerPowerState.Charging;
            }
        }
    }
}
