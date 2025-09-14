using ImmersiveInventorySpoilage.Config;
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

    private void OnInventoryOpened(IPlayer player) => OnTick(1f);

    protected override float Inventory_OnAcquireTransitionSpeed(EnumTransitionType transType, ItemStack stack, float multiplier)
    {
        multiplier = base.Inventory_OnAcquireTransitionSpeed(transType, stack, multiplier);

        EntityBehaviorBodyTemperature tempBehaviour;
        switch (transType)
        {
            case EnumTransitionType.Dry:
                tempBehaviour = PlayerInventory.Player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();


                multiplier = 0.5f - tempBehaviour.Wetness;
                if (!ModConfig.Instance.AllowNegativeDryMultiplier && multiplier < 0) multiplier = 0;

                break;

            case EnumTransitionType.Perish:
                tempBehaviour = PlayerInventory.Player.Entity.GetBehavior<EntityBehaviorBodyTemperature>();
                
                EnumFoodCategory foodCategory = stack.Collectible?.NutritionProps?.FoodCategory ?? EnumFoodCategory.Unknown;
                ModConfig.Instance.WetnessSpoilIncreaseByFoodCat.TryGetValue(foodCategory, out var foodWetnessSpoilRate);
                var wetnessMultiplier = 1 + (tempBehaviour.Wetness * foodWetnessSpoilRate);
                
                multiplier *= wetnessMultiplier;
                break;
        }

        return multiplier;
    }
}
