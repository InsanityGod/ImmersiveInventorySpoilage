using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace ImmersiveInventorySpoilage.Behaviors.Entities;

public class ImmersiveInventorySpoilage(Entity entity) : EntityBehavior(entity)
{
    ImmersivePlayerContainer[]? containers;

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
            new ImmersivePlayerContainer(invBackpack),
            new ImmersivePlayerContainer(mouseBackpack),
            new ImmersivePlayerContainer(hotbarBackpack),
            new ImmersivePlayerContainer(craftinggridBackpack),
        ];
        foreach(var container in containers) container.EarlyInit(entity.Api);
    }

    const float deltaWait = 2.5f;
    float deltaPassed = 0;
    public override void OnGameTick(float deltaTime)
    {
        base.OnGameTick(deltaTime);
        if(containers is null)
        {
            TryInitialize();
            return;
        }

        deltaPassed += deltaTime;
        if (deltaPassed > deltaWait)
        {
            //TODO merge room check
            foreach(var container in containers)
            {
                container.TryUpdateRoom();
            }
            Console.WriteLine("test");
            deltaPassed = 0;
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
