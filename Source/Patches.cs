using AICore;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AIItems;

// LogTool
//
[HarmonyPatch(typeof(LongEventHandler), nameof(LongEventHandler.LongEventsOnGUI))]
public static class LongEventHandler_LongEventsOnGUI_Patch
{
    public static void Postfix()
    {
        LogTool.Log();
    }
}

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

// Add hook to GenerateImageDescription to inject our description or add to our work queue
//
[HarmonyPatch(typeof(CompArt), nameof(CompArt.GenerateImageDescription))]
public static class CompArt_GenerateImageDescription_Patch
{
    private static readonly object _workLock = new();

    public static void Postfix(CompArt __instance, ref TaggedString __result)
    {
        if (__instance?.parent?.ThingID == null)
            throw new ArgumentException("CompArt in patch is null!");
        if (UpdateServerStatus.serverStatusEnum != ServerManager.ServerStatus.Online)
            return;

#if DEBUG
        LogTool.Debug("Handling CompArt type");
#endif

        // Determine if we have AI-Generated result for Thing already
        var thingHash = CityHash.ComputeHash(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(__instance.parent.ThingID)), 32);
        var hashCode = thingHash.AsInt32();

#if DEBUG
        LogTool.Debug($"HashCode: {hashCode}");
#endif

        UpdateItemDescriptions.ItemStatus itemStatus = UpdateItemDescriptions.GetStatus(hashCode);
#if DEBUG
        LogTool.Debug($"ItemStatus: {itemStatus}");
#endif

        switch (itemStatus)
        {
            case UpdateItemDescriptions.ItemStatus.Done:
                (string Title, string Description)? value = UpdateItemDescriptions.GetValues(
                    hashCode
                );
                if (value.HasValue)
                {
#if DEBUG
                    LogTool.Debug(
                        $"Value found: Title = {value.Value.Title}, Description = {value.Value.Description}"
                    );
#endif
                    __instance.titleInt = value.Value.Title;
                    __result = (TaggedString)value.Value.Description;
                    if (ITab_Art.cachedImageSource == __instance)
                    {
#if DEBUG
                        LogTool.Debug("Updating ITab_Art description");
#endif
                        ITab_Art.cachedImageDescription = (TaggedString)value.Value.Description;
                    }
                }
                break;

            case UpdateItemDescriptions.ItemStatus.NotDone:
                lock (_workLock)
                {
#if DEBUG
                    LogTool.Debug("ItemStatus is NotDone. Submitting job.");
#endif
                    // Send thing as job with relevant info
                    // 1. send "Thing" from ScribeSaver.DebugOutputFor()
                    // 2. send Title, Description from CompArt.GenerateTitle(), CompArt.GenerateImageDescription()
                    string myDef = Scribe.saver.DebugOutputFor(__instance.parent);
#if DEBUG
                    LogTool.Debug($"myDef: {myDef}");
#endif
                    TaggedString description = __instance.taleRef.GenerateText(
                        TextGenerationPurpose.ArtDescription,
                        __instance.Props.descriptionMaker
                    );
#if DEBUG
                    LogTool.Debug($"Generated description: {description}");
#endif
                    string title = GenText.CapitalizeAsTitle(
                        __instance.taleRef.GenerateText(
                            TextGenerationPurpose.ArtName,
                            __instance.Props.nameMaker
                        )
                    );
#if DEBUG
                    LogTool.Debug($"Generated title: {title}");
#endif
                    UpdateItemDescriptions.SubmitJob(hashCode, myDef, title, description);
#if DEBUG
                    LogTool.Debug("Job submitted.");
#endif
                }
                break;

            case UpdateItemDescriptions.ItemStatus.Working:
            default:
#if DEBUG
                LogTool.Debug("ItemStatus is Working or default case.");
#endif
                break;
        }
    }
}
