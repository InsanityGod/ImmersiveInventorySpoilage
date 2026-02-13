using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ImmersiveInventorySpoilage.HarmonyPatches;

//TODO check

[HarmonyPatch]
public static class AttachedContainerWorkspacePatches
{
    private sealed class UpdatePosContainer(AttachedContainerWorkspace workspace)
    {
        private long? eventId;

        public void StartUpdatingPosition(IPlayer _ = null)
        {
            if(eventId.HasValue) return;
            UpdateInventoryPos();
            eventId = workspace.WrapperInv.Api.Event.RegisterGameTickListener(UpdateInventoryPos, 500);
        }

        public void UpdateInventoryPos(float deltaTime = 0)
        {
            if(workspace.entity is null)
            {
                StopUpdatingPosition();
                return;
            }

            workspace.WrapperInv.Pos = workspace.entity.Pos.AsBlockPos;
        }

        public void StopUpdatingPosition(IPlayer _ = null)
        {
            if(eventId is null) return;
            
            workspace.WrapperInv.Api.Event.UnregisterGameTickListener(eventId.Value);
        }
    }

    [HarmonyPatch(typeof(AttachedContainerWorkspace), nameof(AttachedContainerWorkspace.TryLoadInv))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchEndForward(
            new CodeMatch(instruction => instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructor && constructor.DeclaringType == typeof(InventoryGeneric))
        );

        matcher.InsertAfter(
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AttachedContainerWorkspacePatches), nameof(PostInventoryConstruction)))
        );

        return matcher.InstructionEnumeration();
    }

    public static void PostInventoryConstruction(InventoryGeneric inventory, AttachedContainerWorkspace workspace)
    {
        var updatePosContainer = new UpdatePosContainer(workspace);

        inventory.OnInventoryOpened += updatePosContainer.StartUpdatingPosition;
        inventory.OnInventoryClosed += updatePosContainer.StopUpdatingPosition;
    }
}
