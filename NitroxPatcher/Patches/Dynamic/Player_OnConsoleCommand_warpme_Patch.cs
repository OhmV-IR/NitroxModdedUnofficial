using System.Reflection;
using NitroxClient.GameLogic;
using NitroxModel.DataStructures;
using NitroxModel.DataStructures.Util;
using NitroxModel.Helper;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Broadcast the escape pod and sub root changes of a player warping back to their escape pod.
/// </summary>
public sealed partial class Player_OnConsoleCommand_warpme_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method((Player t) => t.OnConsoleCommand_warpme());

    public static void Postfix(Player __instance)
    {
        Log.Debug("transpiler hit");
        Optional<NitroxId> currentSubId = Optional.Empty;
        if (__instance.currentSub)
        {
            currentSubId = __instance.currentSub.GetId();
        }

        Optional<NitroxId> currentEscapePodId = Optional.Empty;
        if (__instance.currentEscapePod)
        {
            currentEscapePodId = __instance.currentEscapePod.GetId();
        }

        Resolve<LocalPlayer>().BroadcastSubrootChange(currentSubId);
        Resolve<LocalPlayer>().BroadcastEscapePodChange(currentEscapePodId);
    }
}
