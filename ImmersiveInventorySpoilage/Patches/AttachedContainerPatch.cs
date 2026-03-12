using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace ImmersiveInventorySpoilage.Patches;

[HarmonyPatch]
public static class AttachedContainerPatch
{
    [HarmonyPatch(typeof(AttachedContainerWorkspace), nameof(AttachedContainerWorkspace.TryLoadInv))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);
        var field = AccessTools.Field(typeof(AttachedContainerWorkspace), "wrapperInv");
        matcher.MatchStartForward(
            CodeMatch.StoresField(field)
        );

        matcher.InsertAfter(
            CodeInstruction.LoadArgument(0),
            new CodeInstruction(OpCodes.Ldfld, field),
            CodeInstruction.LoadArgument(3),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AttachedContainerPatch), nameof(AttachImmersiveContainer)))
        );

        return matcher.InstructionEnumeration();
    }

    public static void AttachImmersiveContainer(InventoryBase inventory, Entity entity)
    {
        var immersiveContainer = new ImmersiveContainer(inventory, entity);
        immersiveContainer.EarlyInit(entity.Api);
    }
}
