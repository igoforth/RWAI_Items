using HarmonyLib;

namespace AIItems;
// TODO: Track item lifetimes instead
// * when an item is first found / created
// * which pawn has item and when it changes hands
// * events pawn experiences with item
// TODO: Forward item history to AI Server
//
// References:
// Pawn_JobTracker_StartJob_Patch
// Battle_Add_Patch


// TODO: AI pawn social framework for next mod?
//
// References:
// MoteMaker_ThrowText_Patch
// PawnInteractionsTracker_TryInteractWith_Patch

// anything that requires the map to be defined that we update on periodically
//
// [HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootUpdate))]
// public static partial class GenerallPeriodicMapUpdates_Patch
// {
//     // Dictionary to keep our tasks
//     // format: name, task(update interval where 60000 is one in-game day), do immmediately? (helpful when loading a game)
//     //
//     // Dictionary to keep our tasks
//     private static readonly Dictionary<string, UpdateTask> updateTasks =
//         new()
//         {
//             { "ResourceCount", new UpdateTask(() => 12000, UpdateResources.Task, false) },
//             { "ColonySetting", new UpdateTask(() => 40000, UpdateColonySetting.Task, true) },
//             {
//                 "ColonistOpinions",
//                 new UpdateTask(
//                     () => AIItemsMod.Settings.reportColonistOpinionsFrequency,
//                     ReportColonistOpinions.Task,
//                     AIItemsMod.Settings.reportColonistOpinionsImmediate
//                 )
//             },
//             {
//                 "ColonistThoughts",
//                 new UpdateTask(
//                     () => AIItemsMod.Settings.reportColonistThoughtsFrequency,
//                     ReportColonistThoughts.Task,
//                     AIItemsMod.Settings.reportColonistThoughtsImmediate
//                 )
//             },
//             {
//                 "ColonistRoster",
//                 new UpdateTask(
//                     () => AIItemsMod.Settings.reportColonistRosterFrequency,
//                     UpdateColonistRoster.Task,
//                     AIItemsMod.Settings.reportColonistRosterImmediate
//                 )
//             },
//             {
//                 "EnergyStatus",
//                 new UpdateTask(
//                     () => AIItemsMod.Settings.reportEnergyFrequency,
//                     UpdateEnergyStatus.Task,
//                     AIItemsMod.Settings.reportEnergyImmediate
//                 )
//             },
//             {
//                 "ResearchStatus",
//                 new UpdateTask(
//                     () => AIItemsMod.Settings.reportResearchFrequency,
//                     UpdateResearchStatus.Task,
//                     AIItemsMod.Settings.reportResearchImmediate
//                 )
//             },
//             {
//                 "RoomStatus",
//                 new UpdateTask(
//                     () => AIItemsMod.Settings.reportRoomStatusFrequency,
//                     UpdateRoomStatus.Task,
//                     AIItemsMod.Settings.reportRoomStatusImmediate
//                 )
//             },
//             // Other tasks...
//         };

//     public static void Postfix()
//     {
//         var map = Find.CurrentMap;
//         if (map == null)
//             return;

//         foreach (var taskEntry in updateTasks.ToList())
//         {
//             var key = taskEntry.Key;
//             var task = taskEntry.Value;

//             task.updateTickCounter--;
//             if (task.updateTickCounter < 0)
//             {
//                 task.updateTickCounter = task.updateIntervalFunc();
//                 task.action(map);
//             }

//             updateTasks[key] = task;
//         }
//     }
// }

// TODO: Track with job queue instead
//
[HarmonyPatch(typeof(DesignationManager), nameof(DesignationManager.AddDesignation))]
public static class DesignationManager_AddDesignation_Patch
{
    public static void Postfix(Designation newDes)
    {
        // (string order, string targetLabel) = DesignationHelpers.GetOrderAndTargetLabel(newDes);

        // bail if its a plan, the AI gets confused and thinks we're building stuff when its just planning.  using string because
        // of mods.  it might not be full-proof but should cover most use-cases.
        // if (targetLabel.ToLowerInvariant().Contains("plan"))
        //     return;

        // DesignationQueueManager.EnqueueDesignation(OrderType.Designate, order, targetLabel);
    }
}

// Patch for Designator_Cancel to track when the player issues a cancel
[HarmonyPatch(typeof(Designator_Cancel))]
public static class Designator_Cancel_Patch
{
    [HarmonyPrefix, HarmonyPatch(nameof(Designator_Cancel.DesignateSingleCell))]
    public static void PrefixForDesignateSingleCell(IntVec3 c)
    {
        //Logger.Message($"track cancel cell at  {c}");
        // DesignationHelpers.TrackCancelCell(c);
    }

    [HarmonyPrefix, HarmonyPatch(nameof(Designator_Cancel.DesignateThing))]
    public static void PrefixForDesignateThing(Thing t)
    {
        //Logger.Message($"track cancel thing  {t}");
        // DesignationHelpers.TrackCancelThing(t);
    }
}

// Patch for DesignationManager.RemoveDesignation to process only player-initiated cancellations
[HarmonyPatch(typeof(DesignationManager), nameof(DesignationManager.RemoveDesignation))]
public static class DesignationManager_RemoveDesignation_Patch
{
    public static void Postfix(Designation des)
    {
        // Checks if the action was cancelled for either a Thing or Cell target, bailing out only if neither is found.
        // bool wasCancelledByPlayer =
        //     des.target.Cell != null && DesignationHelpers.IsTrackedCancelCell(des.target.Cell);

        // if (!wasCancelledByPlayer)
        //     return;

        // (string order, string targetLabel) = DesignationHelpers.GetOrderAndTargetLabel(des);

        // // Bail if it's a plan to avoid ChatGPT getting confused.
        // if (targetLabel.ToLowerInvariant().Contains("plan"))
        //     return;

        // DesignationQueueManager.EnqueueDesignation(OrderType.Cancel, order, targetLabel);
    }
}

// on every tick, regardless if game is paused or not
[HarmonyPatch(typeof(TickManager), "DoSingleTick")]
public static class TickManager_DoSingleTick_Patch
{
    public static void Postfix()
    {
        Map map = Find.CurrentMap;
        if (map == null)
        {
            return;
        }
        // DesignationQueueManager.Update();
    }
}
