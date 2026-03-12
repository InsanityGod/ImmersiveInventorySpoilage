using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace ImmersiveInventorySpoilage.Behaviors.Entities;

public class ImmersiveInventorySpoilage(Entity entity) : EntityBehavior(entity)
{
    ImmersiveContainer[]? containers;

    public override void AfterInitialized(bool onFirstSpawn)
    {
        base.AfterInitialized(onFirstSpawn);
        TryInitialize();
    }

    private void TryInitialize()
    {
        if(entity is not EntityPlayer playerEntity || playerEntity.Player is not IPlayer player) return; //TODO warn?

        if (player.InventoryManager.GetInventory($"backpack-{player.PlayerUID}") is not InventoryBasePlayer invBackpack) return;
        if (player.InventoryManager.GetInventory($"mouse-{player.PlayerUID}") is not InventoryBasePlayer mouseBackpack) return;
        if (player.InventoryManager.GetInventory($"hotbar-{player.PlayerUID}") is not InventoryBasePlayer hotbarBackpack) return;
        if (player.InventoryManager.GetInventory($"craftinggrid-{player.PlayerUID}") is not InventoryBasePlayer craftinggridBackpack) return;

        containers = [
            new ImmersiveContainer(invBackpack, playerEntity),
            new ImmersiveContainer(mouseBackpack, playerEntity),
            new ImmersiveContainer(hotbarBackpack, playerEntity),
            new ImmersiveContainer(craftinggridBackpack, playerEntity),
        ];
        foreach(var container in containers) container.EarlyInit(entity.Api);
    }

    public override void OnGameTick(float deltaTime)
    {
        base.OnGameTick(deltaTime);
        if(containers is null)
        {
            TryInitialize();
        }
    }

    public override void OnEntityDespawn(EntityDespawnData despawn)
    {
        if(containers is not null)
        {
            foreach (var container in containers) 
            {
                container.Unlink();
            }
        }

        base.OnEntityDespawn(despawn);
    }

    public override string PropertyName() => nameof(ImmersiveInventorySpoilage);
}
