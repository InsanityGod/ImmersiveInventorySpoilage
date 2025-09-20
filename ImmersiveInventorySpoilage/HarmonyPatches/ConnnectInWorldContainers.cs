using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ImmersiveInventorySpoilage.HarmonyPatches;

[HarmonyPatch]
public static class ConnnectInWorldContainers
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods() => typeof(InventoryBasePlayer).GetConstructors();

    public static readonly ConditionalWeakTable<InventoryBasePlayer, ImmersivePlayerContainer> ImmersiveContainers = new();

    [HarmonyPostfix]
    public static void PostFix(InventoryBase __instance, ICoreAPI api)
    {
        if(__instance is not InventoryBasePlayer playerInventory || ImmersiveContainers.TryGetValue(playerInventory, out _)) return;

        var inWorldContainer = new ImmersivePlayerContainer(playerInventory);
        inWorldContainer.EarlyInit(api);
        ImmersiveContainers.Add(playerInventory, inWorldContainer);
    }
}