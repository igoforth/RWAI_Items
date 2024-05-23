using HarmonyLib;

namespace AIItems;

using System.Collections.Generic;
using System.Linq;

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
