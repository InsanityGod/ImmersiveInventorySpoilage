using HarmonyLib;
using ImmersiveInventorySpoilage.Behaviors.Items;
using ImmersiveInventorySpoilage.HarmonyPatches;
using InsanityLib.Attributes.Auto;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: AutoRegistry("immersiveinventoryspoilage")]
namespace ImmersiveInventorySpoilage
{
    public class ImmersiveInventorySpoilageModSystem : ModSystem
    {

        private Harmony harmony;

        public override void Start(ICoreAPI api)
        {
            api.Event.RegisterGameTickListener(UpdateRooms, 1000);

            if (Harmony.HasAnyPatches(Mod.Info.ModID)) return;
            
            harmony = new Harmony(Mod.Info.ModID);

            harmony.PatchAllUncategorized();
        }

        public static void UpdateRooms(float deltaTime)
        {
            foreach(var pair in ConnnectInWorldContainers.ImmersiveContainers)
            {
                pair.Value?.ReloadRoom();
            }
        }

        public override void StartServerSide(ICoreServerAPI api) => WetObject.RegisterListener(api);

        public override void Dispose()
        {
            harmony?.UnpatchAll(Mod.Info.ModID);
        }
    }
}