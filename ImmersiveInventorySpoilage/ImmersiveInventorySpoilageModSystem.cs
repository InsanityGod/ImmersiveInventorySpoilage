using ImmersiveInventorySpoilage.Behaviors.Items;
using ImmersiveInventorySpoilage.HarmonyPatches;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ImmersiveInventorySpoilage;

public partial class ImmersiveInventorySpoilageModSystem : ModSystem
{

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        AutoSetup(api);
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        GameTickListenerIds.Add(api.Event.RegisterGameTickListener(UpdateRooms, 1000));
    }

    public static void UpdateRooms(float deltaTime)
    {
        foreach(var pair in ConnnectInWorldContainers.ImmersiveContainers)
        {
            pair.Value?.TryUpdateRoom();
        }
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        GameTickListenerIds.Add(WetObject.RegisterListener(api));
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);
        AutoAssetsLoaded(api);
    }

    public override void Dispose()
    {
        base.Dispose();
        AutoDispose();
    }
}