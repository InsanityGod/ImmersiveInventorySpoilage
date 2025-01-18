using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace ImmersiveInventorySpoilage.Config
{
    public class ModConfig
    {
        /// <summary>
        /// How large the position aware perish rate effect is on the player inventory.
        /// Must be between 0 and 1, 1 meaning that temperature/room/light affects the player inventory just as much as any other inventory (such as chests) while 0 means it isn't affected at all
        /// </summary>
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float PositionAwarePerishRateSimularity { get; set; } = 1;

        /// <summary>
        /// Whether the drying multiplier is allowed to go negative.
        /// If set to true drying process may start reversing if you get too wet.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowNegativeDryMultiplier { get; set; } = true;

        /// <summary>
        /// Wether item wetness is a thing
        /// If set to true, certain items (such as snow) may make you wet if you carry them
        /// </summary>
        [DefaultValue(true)]
        public bool Feature_ItemWetness { get; set; } = true;

        /// <summary>
        /// Wether stuff (for now only slush) can continue to melt all the way until it turns into water (and slips through the backpack, effictively removing it from the inventory)
        /// </summary>
        [DefaultValue(true)]
        public bool Feature_StuffCanFullyMelt { get; set; } = true;

        /// <summary>
        /// Up to how much the spoil rate multiplier increases when wet
        /// You can remove a food category (or set it to 0) to disable the effect on it
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
}