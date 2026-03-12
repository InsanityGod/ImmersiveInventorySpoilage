using ImmersiveInventorySpoilage.Config;
using System;
using System.Diagnostics;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ImmersiveInventorySpoilage;

public class ImmersiveContainer(InventoryBase inventory, Entity entity) : InWorldContainer(() => inventory, string.Empty)
{
    public readonly EntityBehaviorBodyTemperature? tempBehavior = entity.GetBehavior<EntityBehaviorBodyTemperature>();
    public readonly Entity entity = entity;
    public readonly InventoryBase inventory = inventory;

    private long lastRoomUpdateTimestamp = 0;
    private const double RefreshThresholdSeconds = 2.5;
    
    public BlockPos GetPosition() => entity?.Pos?.AsBlockPos ?? inventory.Pos;
    
    private void OnRequireSyncToClient()
    {
        //Empty placehold as base class requires it but we don't need it
    }

    public void EarlyInit(ICoreAPI api)
    {
        Api = api;
        positionProvider = GetPosition;
        onRequireSyncToClient = OnRequireSyncToClient;
        roomReg = Api.ModLoader.GetModSystem<RoomRegistry>();

		inventory.OnAcquireTransitionSpeed += Inventory_OnAcquireTransitionSpeed;
		inventory.OnInventoryOpened += OnInventoryOpened;
    }

    public void Unlink()
    {
        inventory.OnAcquireTransitionSpeed -= Inventory_OnAcquireTransitionSpeed;
		inventory.OnInventoryOpened -= OnInventoryOpened;
    }

    private void OnInventoryOpened(IPlayer player) => OnTick(1f);

    protected override float Inventory_OnAcquireTransitionSpeed(EnumTransitionType transType, ItemStack stack, float baseMul)
    {
        if(Stopwatch.GetElapsedTime(lastRoomUpdateTimestamp).TotalSeconds > RefreshThresholdSeconds)
        {
            TryUpdateRoom();
            lastRoomUpdateTimestamp = Stopwatch.GetTimestamp();
        }

        try
        {
            baseMul = base.Inventory_OnAcquireTransitionSpeed(transType, stack, baseMul);

            switch (transType)
            {
                case EnumTransitionType.Dry:
                    if(tempBehavior is not null)
                    {
                        baseMul = 0.5f - tempBehavior.Wetness;
                        if (!ModConfig.Instance.AllowNegativeDryMultiplier && baseMul < 0) baseMul = 0;
                    }

                    break;

                case EnumTransitionType.Perish:
                    if(tempBehavior is not null)
                    {
                        EnumFoodCategory foodCategory = stack.Collectible?.NutritionProps?.FoodCategory ?? EnumFoodCategory.Unknown;
                        ModConfig.Instance.WetnessSpoilIncreaseByFoodCat.TryGetValue(foodCategory, out var foodWetnessSpoilRate);
                        var wetnessMultiplier = 1 + (tempBehavior.Wetness * foodWetnessSpoilRate);
                        
                        baseMul *= wetnessMultiplier;
                    }
                    break;
            }
        }
        catch(Exception ex)
        {
            entity?.Api.Logger.Error("[immersiveinventoryspoilage] an error occurred during {0} for inventory '{1}': {2}", nameof(Inventory_OnAcquireTransitionSpeed), inventory.InventoryID, ex);
        }
        return baseMul;
    }

    public void TryUpdateRoom()
    {
        try
        {
            var pos = GetPosition();
            if(pos is null) return;

            room = roomReg.GetRoomForPosition(pos);
        }
        catch(Exception ex)
        {
            inventory?.Api.Logger.Error("[immersiveinventoryspoilage] an error occurred during {0} for inventory '{1}': {2}", nameof(TryUpdateRoom), inventory.InventoryID, ex);
        }
    }
}
