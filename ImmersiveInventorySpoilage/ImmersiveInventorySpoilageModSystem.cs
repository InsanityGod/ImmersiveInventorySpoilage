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
        private const string ConfigName = "ImmersiveFoodSpoilageConfig.json";

        private Harmony harmony;

        public static Config Config { get; private set; }

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

            try
            {
                Config ??= api.LoadModConfig<Config>(ConfigName);
                if (Config == null)
                {
                    Config = new();
                    api.StoreModConfig(Config, ConfigName);
                }
            }
            catch (Exception ex)
            {
                api.Logger.Error(ex);
                api.Logger.Warning("Failed to load config, using default values instead");
                Config = new();
            }
        }

        public override void Dispose()
        {
            harmony?.UnpatchAll(Mod.Info.ModID);
            Config = null;
        }
    }
}