using ImmersiveInventorySpoilage.Behaviors.Properties;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace ImmersiveInventorySpoilage.Behaviors.Items;

public class WetObject(CollectibleObject collObj) : CollectibleBehavior(collObj)
{
    public WetObjectProperties WetObjectProps { get; protected set; }

    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);
        WetObjectProps = properties.AsObject<WetObjectProperties>(null, collObj.Code.Domain) ?? new();
    }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        dsc.AppendLine(Lang.Get("immersiveinventoryspoilage:wetnesstooltip"));
    }

    public void ApplyWetnessEffect(ItemStack item, IPlayer player, float secondsPassed)
    {
        var tempBehaviour = player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();

        //This is to prevent the wetness from exceeding the normal maximum value of 1 while still allowing other mods to go past it if they want
        var maxWetnessIncrease = 1 - tempBehaviour.Wetness;
        if (maxWetnessIncrease <= 0) return;

        tempBehaviour.Wetness += Math.Max(item.StackSize * WetObjectProps.ItemWetnessFactor * secondsPassed, maxWetnessIncrease);
    }

    public static long RegisterListener(ICoreAPI api) => api.Event.RegisterGameTickListener(secondsPassed =>
    {
        foreach (var player in api.World.AllOnlinePlayers)
        {
            var serverPlayer = player as ServerPlayer;
            if (serverPlayer.ConnectionState != Vintagestory.API.Server.EnumClientState.Connected && serverPlayer.ConnectionState != Vintagestory.API.Server.EnumClientState.Playing) continue;

            player.Entity.WalkInventory(invSlot =>
            {
                //Only run this on player inventory
                if (invSlot.Inventory is not InventoryBasePlayer || invSlot.Empty) return true;

                invSlot.Itemstack.Collectible.GetBehavior<WetObject>()?.ApplyWetnessEffect(invSlot.Itemstack, player, secondsPassed);
                return true;
            });
        }
    }, 1000);
}