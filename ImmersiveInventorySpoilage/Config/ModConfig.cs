using InsanityLib.Attributes.Auto.Config;
using System.Collections.Generic;
using System.ComponentModel;
using Vintagestory.API.Common;

namespace ImmersiveInventorySpoilage.Config;

public class ModConfig
{
    [AutoConfig("ImmersiveInventorySpoilageConfig.json", ServerSync = true)]
    public static ModConfig Instance { get; set; }

    [DefaultValue(true)]
    public bool AllowNegativeDryMultiplier { get; set; } = true;

    /// <summary>
    /// If set to true, items such as slush will be considered 'wet' and will make the player wetter when in inventory
    /// </summary>
    [DefaultValue(true)]
    public bool ItemWetness { get; set; } = true;

    /// <summary>
    /// If set to true, items such as slush can fully melt into water (at which point the inventory can no longer contain it)
    /// </summary>
    [DefaultValue(true)]
    public bool StuffCanFullyMelt { get; set; } = true;

    /// <summary>
    /// Multiplier relative to Wetness that increases spoil rate
    /// You can remove a food category (or set it to 0) to disable the effect on it
    /// (rate = originalRate * (1 + (WetnessPercentage * WetnessSpoilIncreaseByFoodCat))
    /// </summary>
    public Dictionary<EnumFoodCategory, float> WetnessSpoilIncreaseByFoodCat { get; set; } = new Dictionary<EnumFoodCategory, float>
    {
        { EnumFoodCategory.NoNutrition, 0.2f },
        { EnumFoodCategory.Fruit, 0.2f },
        { EnumFoodCategory.Vegetable, 0.2f },
        { EnumFoodCategory.Protein, 0.4f },
        { EnumFoodCategory.Grain, 0.8f },
        { EnumFoodCategory.Dairy, 1f },
        { EnumFoodCategory.Unknown, 0.2f }
    };
}