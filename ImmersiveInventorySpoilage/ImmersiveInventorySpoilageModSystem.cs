using HarmonyLib;
using ImmersiveInventorySpoilage.HarmonyPatches;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Common;
using Vintagestory.Server;

namespace ImmersiveInventorySpoilage
{
    public class ImmersiveInventorySpoilageModSystem : ModSystem
    {
        private Harmony harmony;

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api)
        {
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                var invType = typeof(InventoryBasePlayer);

                var invTypeCtor1 = AccessTools.Constructor(invType, new Type[] { typeof(string), typeof(string), typeof(ICoreAPI) });
                harmony.Patch(invTypeCtor1, postfix: new HarmonyMethod(typeof(PatchInventoryBasePlayerClass).GetMethod(nameof(PatchInventoryBasePlayerClass.PostFix))));

                var invTypeCtor2 = AccessTools.Constructor(invType, new Type[] { typeof(string), typeof(ICoreAPI) });
                harmony.Patch(invTypeCtor2, postfix: new HarmonyMethod(typeof(PatchInventoryBasePlayerClass).GetMethod(nameof(PatchInventoryBasePlayerClass.PostFix))));
            }
        }

        public override void Dispose() => harmony?.UnpatchAll(Mod.Info.ModID);
    }
}