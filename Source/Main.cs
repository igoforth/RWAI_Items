global using System;
global using RimWorld;
global using UnityEngine;
global using Verse;
using HarmonyLib;

namespace AIItems;

using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

[HarmonyPatch(typeof(Current), nameof(Current.Notify_LoadedSceneChanged))]
[StaticConstructorOnStartup]
public static class Main
{
    private static readonly ConcurrentQueue<Action> actions = new();

    static Main()
    {
        Postfix();
    }

    public static void Postfix()
    {
        if (GenScene.InEntryScene)
        {
            _ = Current.Root_Entry.StartCoroutine(Process());
        }

        if (GenScene.InPlayScene)
        {
            _ = Current.Root_Play.StartCoroutine(Process());
        }
    }

    private static IEnumerator Process()
    {
        while (true)
        {
            yield return null;
            if (!actions.TryDequeue(out Action action))
            {
                continue;
            }

            action();
        }
    }

    public static async Task Perform(Action action)
    {
        bool working = true;
        actions.Enqueue(() =>
        {
            action();
            working = false;
        });
        while (working)
        {
            await Task.Delay(200);
        }
    }

    public static async Task<T> Perform<T>(Func<T> action)
    {
        T result = default;
        bool working = true;
        actions.Enqueue(() =>
        {
            result = action();
            working = false;
        });
        while (working)
        {
            await Task.Delay(200);
        }

        return result;
    }
}

public class AIItemsMod : Mod
{
    public static CancellationTokenSource onQuit = new();
    public static AIItemsSettings Settings;
    public static Mod self;

    public AIItemsMod(ModContentPack content)
        : base(content)
    {
        self = this;
        Settings = GetSettings<AIItemsSettings>();

        Harmony harmony = new("net.trojan.rimworld.mod.AICore");
        harmony.PatchAll();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            // This performs any necessary setup when the game is loaded


            // Personas.UpdateVoiceInformation();
            // Tools.ReloadGPTModels();
            if (Settings.IsConfigured)
            {
                // This is the main entry point for the mod

                // Tools.UpdateApiConfigs();
                // Personas.Add("Player has launched Rimworld and is on the start screen", 0);
            }
        });

        Application.wantsToQuit += () =>
        {
            onQuit.Cancel();
            return true;
        };
    }

    public static bool Running => !onQuit.IsCancellationRequested;

    public override void DoSettingsWindowContents(Rect inRect) => Settings.DoWindowContents(inRect);

    public override string SettingsCategory() => "AI Items";
}
