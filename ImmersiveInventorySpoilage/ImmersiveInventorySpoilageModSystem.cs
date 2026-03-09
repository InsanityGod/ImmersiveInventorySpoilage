using ImmersiveInventorySpoilage.Behaviors.Items;
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