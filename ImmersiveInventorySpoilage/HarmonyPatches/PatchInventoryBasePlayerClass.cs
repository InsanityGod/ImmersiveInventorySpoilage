using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace ImmersiveInventorySpoilage.HarmonyPatches
{
    public static class PatchInventoryBasePlayerClass
    {
        public static void PostFix(InventoryBasePlayer __instance)
        {
            __instance.OnAcquireTransitionSpeed = (EnumTransitionType transType, ItemStack stack, float baseMul) =>
            {
                if (__instance.Player == null)
                {
                    return baseMul;
                }

                float positionAwarePerishRate = 1;

                if (__instance.Api != null && transType == EnumTransitionType.Perish)
                {
                    //START GetPerishRate

                    var pos = __instance.Player?.Entity?.Pos?.AsBlockPos ?? __instance.Pos;

                    if (pos != null && ImmersiveInventorySpoilageModSystem.Config.PositionAwarePerishRateSimularity != 0)
                    {
                        BlockPos sealevelpos = pos.Copy();
                        sealevelpos.Y = __instance.Api.World.SeaLevel;

                        //TODO maybe cache this value for a few ticks for performance
                        float temperature = __instance.Api.World.BlockAccessor.GetClimateAt(sealevelpos, EnumGetClimateMode.ForSuppliedDate_TemperatureOnly, __instance.Api.World.Calendar.TotalDays).Temperature;

                        //TODO maybe cache this value for a few ticks for performance
                        var room = __instance.Api.ModLoader.GetModSystem<RoomRegistry>().GetRoomForPosition(pos);

                        float soilTempWeight = 0f;
                        float skyLightProportion = (float)room.SkylightCount / (float)Math.Max(1, room.SkylightCount + room.NonSkylightCount);

                        if (room.IsSmallRoom)
                        {
                            soilTempWeight = 1f;
                            soilTempWeight -= 0.4f * skyLightProportion;
                            soilTempWeight -= 0.5f * GameMath.Clamp((float)room.NonCoolingWallCount / (float)Math.Max(1, room.CoolingWallCount), 0f, 1f);
                        }

                        int lightlevel = __instance.Api.World.BlockAccessor.GetLightLevel(pos, EnumLightLevelType.OnlySunLight);
                        float lightImportance = 0.1f;

                        if (room.IsSmallRoom)
                        {
                            lightImportance += 0.3f * soilTempWeight + 1.75f * skyLightProportion;
                        }
                        else if ((float)room.ExitCount <= 0.1f * (float)(room.CoolingWallCount + room.NonCoolingWallCount))
                        {
                            lightImportance += 1.25f * skyLightProportion;
                        }
                        else
                        {
                            lightImportance += 0.5f * skyLightProportion;
                        }

                        lightImportance = GameMath.Clamp(lightImportance, 0f, 1.5f);

                        float airTemp = temperature + (float)GameMath.Clamp(lightlevel - 11, 0, 10) * lightImportance;
                        float cellarTemp = 5f;
                        float hereTemp = GameMath.Lerp(airTemp, cellarTemp, soilTempWeight);
                        hereTemp = Math.Min(hereTemp, airTemp);
                        positionAwarePerishRate = Math.Max(0.1f, Math.Min(2.4f, (float)Math.Pow(3.0, (double)(hereTemp / 19f) - 1.2) - 0.1f));
                    }
                    //END GetPerishRate

                    var tempBehaviour = __instance.Player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();

                    positionAwarePerishRate += tempBehaviour.Wetness;
                }
                else if (transType == EnumTransitionType.Dry)
                {
                    var tempBehaviour = __instance.Player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();
                    positionAwarePerishRate = Math.Max(0.5f - tempBehaviour.Wetness, -0.5f);
                }
                else if (transType == EnumTransitionType.Melt)
                {
                    positionAwarePerishRate = 0.25f;
                }

                //Apply config
                positionAwarePerishRate = 1 + (positionAwarePerishRate - 1) * ImmersiveInventorySpoilageModSystem.Config.PositionAwarePerishRateSimularity;

                return baseMul * positionAwarePerishRate;
            };
        }
    }
}