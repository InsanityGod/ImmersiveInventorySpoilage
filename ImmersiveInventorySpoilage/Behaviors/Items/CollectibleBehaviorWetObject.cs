using ImmersiveInventorySpoilage.Config.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace ImmersiveInventorySpoilage.Behaviors.Items
{
    public class CollectibleBehaviorWetObject : CollectibleBehavior
    {
        public CollectibleBehaviorWetObject(CollectibleObject collObj) : base(collObj)
        {
        }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            WetObjectProps = properties.AsObject<WetObjectProperties>(null, collObj.Code.Domain) ?? new();
        }

        public WetObjectProperties WetObjectProps { get; protected set; }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            dsc.AppendLine("<font color=\"#0E87CC\">Carrying this item will make you wet!</font>");
        }

        public void ApplyWetnessEffect(ItemStack item, IPlayer player, float secondsPassed)
        {
            var tempBehaviour = player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();

            //This is to prevent the wetness from exceeding the normal maximum value of 1 while still allowing other mods to go past it if they want
            var maxWetnessIncrease = 1 - tempBehaviour.Wetness;
            if (maxWetnessIncrease <= 0) return;

            tempBehaviour.Wetness += Math.Max(item.StackSize * WetObjectProps.ItemWetnessFactor * secondsPassed, maxWetnessIncrease);
        }

        public static void RegisterListener(ICoreAPI api)
        {
            if (api.Side == EnumAppSide.Server)
            {
                api.Event.RegisterGameTickListener(secondsPassed =>
                {
                    foreach (var player in api.World.AllOnlinePlayers)
                    {
                        var serverPlayer = player as ServerPlayer;
                        if (serverPlayer.ConnectionState != Vintagestory.API.Server.EnumClientState.Connected && serverPlayer.ConnectionState != Vintagestory.API.Server.EnumClientState.Playing)
                        {
                            continue;
                        }

                        player.Entity.WalkInventory(invSlot =>
                        {
                            //Only run this on player inventory
                            if (invSlot.Inventory is not InventoryBasePlayer) return true;

                            invSlot.Itemstack?.Collectible?.GetBehavior<CollectibleBehaviorWetObject>()?.ApplyWetnessEffect(invSlot.Itemstack, player, secondsPassed);
                            return true;
                        });
                    }
                }, 1000);
            }
        }
    }
}