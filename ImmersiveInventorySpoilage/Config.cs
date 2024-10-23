using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersiveInventorySpoilage
{
    public class Config
    {
        /// <summary>
        /// How large the position aware perish rate effect is on the player inventory.
        /// Must be between 0 and 1, 1 meaning that temperature/room/light affects the player inventory just as much as any other inventory (such as chests) while 0 means it isn't affected at all
        /// </summary>
        [Range(0f, 1f)]
        public float PositionAwarePerishRateSimularity { get; set; } = 1;

        /// <summary>
        /// Whether the drying multiplier is allowed to go negative.
        /// If set to true drying process may start reversing if you get too wet.
        /// </summary>
        public bool AllowNegativeDryMultiplier { get; set; } = true;

        /// <summary>
        /// Up to how much the spoil rate multiplier increases when wet
        /// (since wetness can only go up to 1, this is value is simply multiplied with the wetness to get the multiplier increase)
        /// </summary>
        public float MaxWetnessSpoilRateIncrease { get; set; } = 0.25f;
    }
}