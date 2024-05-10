using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AIItems;

// TODO: Track item lifetimes instead
// * when an item is first found / created
// * which pawn has item and when it changes hands
// * events pawn experiences with item
//
[HarmonyPatch]
public static class Pawn_JobTracker_StartJob_Patch
{
    static MethodBase TargetMethod()
    {
        var (type, name) = (typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob));
        var args1 = new Type[]
        {
            typeof(Job),
            typeof(JobCondition),
            typeof(ThinkNode),
            typeof(bool),
            typeof(bool),
            typeof(ThinkTreeDef),
            typeof(JobTag?),
            typeof(bool),
            typeof(bool),
            typeof(bool?),
            typeof(bool),
            typeof(bool)
        };
        var args2 = args1.AddItem(typeof(bool)).ToArray();
        return AccessTools.Method(type, name, args1) ?? AccessTools.Method(type, name, args2);
    }

    static void Handle(Pawn_JobTracker tracker, JobDriver curDriver)
    {
        tracker.curDriver = curDriver;

        var pawn = tracker.pawn;
        if (pawn == null || pawn.AnimalOrWildMan())
            return;

        var job = curDriver.job;
        if (job == null)
            return;

        var workType = job.workGiverDef?.workType;
        if (workType == WorkTypeDefOf.Hauling)
            return;
        if (workType == WorkTypeDefOf.Construction)
            return;
        if (workType == WorkTypeDefOf.PlantCutting)
            return;
        if (workType == WorkTypeDefOf.Mining)
            return;
        if (workType == Defs.Cleaning)
            return;

        var defName = job.def.defName;
        if (defName == null)
            return;
        if (defName.StartsWith("Wait"))
            return;
        if (defName.StartsWith("Goto"))
            return;

        var report = curDriver.GetReport();
        report = report.Replace(pawn.LabelShortCap, pawn.NameAndType());
        if (job.targetA.Thing is Pawn target)
            report = report.Replace(target.LabelShortCap, target.NameAndType());

        Personas.Add($"{pawn.NameAndType()} {report}", 3);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchStartForward(
                new CodeMatch(
                    CodeInstruction.StoreField(
                        typeof(Pawn_JobTracker),
                        nameof(Pawn_JobTracker.curDriver)
                    )
                )
            )
            .SetInstruction(CodeInstruction.Call(() => Handle(null, null)))
            .Instructions();
    }
}

// TODO: Forward item history to AI Server
//
[HarmonyPatch]
public static class Battle_Add_Patch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return SymbolExtensions.GetMethodInfo(() => new Battle().Add(null));
        yield return SymbolExtensions.GetMethodInfo(() => new PlayLog().Add(null));
    }

    public static void Postfix(LogEntry entry)
    {
        string text;
        Tools.ExtractPawnsFromLog(entry, out var from, out var to);
        text = entry.ToGameStringFromPOVWithType(from);
        if (text != null)
            Personas.Add(text, 1);
        text = entry.ToGameStringFromPOVWithType(to);
        if (text != null)
            Personas.Add(text, 1);
    }
}

// TODO: Future Pawn social tracker
//
[HarmonyPatch(typeof(MoteMaker), nameof(MoteMaker.ThrowText))]
[HarmonyPatch(new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) })]
public static class MoteMaker_ThrowText_Patch
{
    public static void Postfix(Vector3 loc, Map map, string text)
    {
        var pawns = map.mapPawns.FreeColonistsSpawned.Where(pawn =>
            (pawn.DrawPos - loc).MagnitudeHorizontalSquared() < 4f
        );
        if (pawns.Count() != 1)
            return;
        var pawn = pawns.First();
        text = text.Replace("\n", " ");
        Personas.Add($"{pawn.NameAndType()}: \"{text}\"", 0);
    }
}

// TODO: Future Pawn social tracker
//
[HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
[HarmonyPatch([typeof(Pawn), typeof(InteractionDef)])]
public static class Pawn_InteractionsTracker_TryInteractWith_Patch
{
    public static void Postfix(Pawn recipient, Pawn ___pawn, bool __result, InteractionDef intDef)
    {
        if (__result == false)
            return;

        // at least one pawn should be of the player's faction
        if (___pawn.Faction != Faction.OfPlayer && recipient.Faction != Faction.OfPlayer)
            return;

        var opinionOfRecipient = ___pawn.relations.OpinionOf(recipient);
        var opinionOfPawn = recipient.relations.OpinionOf(___pawn);

        // construct a message that includes the type and name of each pawn
        var pawnType = GetPawnType(___pawn);
        var recipientType = GetPawnType(recipient);
        var message =
            $"{pawnType} '{___pawn.Name.ToStringShort}' interacted with {recipientType} '{recipient.Name.ToStringShort}'. "
            + $"Opinions --- {___pawn.Name.ToStringShort}'s opinion of {recipient.Name.ToStringShort}: {opinionOfRecipient}, "
            + $"{recipient.Name.ToStringShort}'s opinion of {___pawn.Name.ToStringShort}: {opinionOfPawn}. "
            + $"Interaction: '{intDef?.label ?? "something"}' initiated by {___pawn.Name.ToStringShort}.";

        Personas.Add(message, 2);
    }

    // this could be expanded with more types or different logic to determine the type
    //
    public static string GetPawnType(Pawn pawn)
    {
        if (pawn.IsColonist)
            return "colonist";
        if (pawn.IsPrisoner)
            return "prisoner";
        if (pawn.IsSlave)
            return "slave";
        return "visitor";
    }
}
