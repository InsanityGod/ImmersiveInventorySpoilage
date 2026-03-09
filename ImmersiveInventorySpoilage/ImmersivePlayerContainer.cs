using ImmersiveInventorySpoilage.Config;
using InsanityLib.Util;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ImmersiveInventorySpoilage;

public class ImmersivePlayerContainer(InventoryBasePlayer playerInvetory) : InWorldContainer(() => playerInvetory, string.Empty)
{
    public readonly InventoryBasePlayer PlayerInventory = playerInvetory;
    
    public BlockPos GetPosition() => PlayerInventory.Pos = PlayerInventory.Player?.Entity?.Pos.AsBlockPos ?? PlayerInventory.Pos;
    
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

		PlayerInventory.OnAcquireTransitionSpeed += Inventory_OnAcquireTransitionSpeed;
		PlayerInventory.OnInventoryOpened += OnInventoryOpened;
    }

    public void Unlink()
    {
        PlayerInventory.OnAcquireTransitionSpeed -= Inventory_OnAcquireTransitionSpeed;
		PlayerInventory.OnInventoryOpened -= OnInventoryOpened;
    }

    private void OnInventoryOpened(IPlayer player) => OnTick(1f);

    protected override float Inventory_OnAcquireTransitionSpeed(EnumTransitionType transType, ItemStack stack, float baseMul)
    {
        try
        {
            baseMul = base.Inventory_OnAcquireTransitionSpeed(transType, stack, baseMul);

            EntityBehaviorBodyTemperature tempBehaviour;
            switch (transType)
            {
                case EnumTransitionType.Dry:
                    tempBehaviour = PlayerInventory.Player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();


                    baseMul = 0.5f - tempBehaviour.Wetness;
                    if (!ModConfig.Instance.AllowNegativeDryMultiplier && baseMul < 0) baseMul = 0;

                    break;

                case EnumTransitionType.Perish:
                    tempBehaviour = PlayerInventory.Player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();
                    
                    EnumFoodCategory foodCategory = stack.Collectible?.NutritionProps?.FoodCategory ?? EnumFoodCategory.Unknown;
                    ModConfig.Instance.WetnessSpoilIncreaseByFoodCat.TryGetValue(foodCategory, out var foodWetnessSpoilRate);
                    var wetnessMultiplier = 1 + (tempBehaviour.Wetness * foodWetnessSpoilRate);
                    
                    baseMul *= wetnessMultiplier;
                    break;
            }
        }
        catch(Exception ex)
        {
            PlayerInventory?.Api.Logger.Error("[immersiveinventoryspoilage] an error occurred during {0}: {1}", nameof(Inventory_OnAcquireTransitionSpeed), ex);
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
            PlayerInventory?.Api.Logger.Error("[immersiveinventoryspoilage] an error occurred during {0}: {1}", nameof(TryUpdateRoom), ex);
        }
    }
}
