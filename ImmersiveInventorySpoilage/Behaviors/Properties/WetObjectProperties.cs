namespace ImmersiveInventorySpoilage.Behaviors.Properties;

public class WetObjectProperties
{
    /// <summary>
    /// This is multiplied with the amount of items and the time (in seconds) passed to calculate the increase in wetness.
    /// (for reference 0 is dry and 1 is soaking wet)
    /// (Default value is 0.00026f and would mean that a stack of 64 items would make you fully wet in 60 seconds)
    /// </summary>
    public float ItemWetnessFactor { get; set; } = 0.00052f;
}