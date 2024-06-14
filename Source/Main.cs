using System.Collections;
using System.Collections.Concurrent;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace AIItems;

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
            if (actions.TryDequeue(out Action? action))
            {
                action?.Invoke();
            }
        }
    }

    public static async Task Perform(Action action)
    {
        TaskCompletionSource<bool> tcs = new();

        actions.Enqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        // Process the actions in the queue
        while (!tcs.Task.IsCompleted)
        {
            if (actions.TryDequeue(out Action? queuedAction))
            {
                queuedAction();
            }

            await Task.Delay(200).ConfigureAwait(false);
        }

        _ = await tcs.Task.ConfigureAwait(false);
    }

    public static async Task<T> Perform<T>(Func<T> action)
    {
        TaskCompletionSource<T> tcs = new();

        actions.Enqueue(() =>
        {
            try
            {
                T? result = action();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        // Process the actions in the queue
        while (!tcs.Task.IsCompleted)
        {
            if (actions.TryDequeue(out Action? queuedAction))
            {
                queuedAction();
            }

            await Task.Delay(200).ConfigureAwait(false);
        }

        return await tcs.Task.ConfigureAwait(false);
    }
}

public class AIItemsMod : Mod
{
    public static CancellationTokenSource onQuit = new();
    public static AIItemsSettings? Settings;
    public static Mod? self;

    static AIItemsMod() { }

    public AIItemsMod(ModContentPack content)
        : base(content)
    {
#if DEBUG
        LogTool.Debug("AIItemsMod: Constructor called");
#endif

        self = this;
        Settings = GetSettings<AIItemsSettings>();

        Harmony harmony = new("net.trojan.rimworld.mod.AIItems");
        harmony.PatchAll();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            if (Settings.IsConfigured) { }
        });

        Application.wantsToQuit += () =>
        {
            onQuit.Cancel();
            return true;
        };
    }

    public static bool Running => !onQuit.IsCancellationRequested;

    public override void DoSettingsWindowContents(Rect inRect) =>
        AIItemsSettings.DoWindowContents(inRect);

    public override string SettingsCategory() => "";
}
