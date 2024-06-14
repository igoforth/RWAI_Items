using UnityEngine;
using Verse;

namespace AIItems;

public partial class AIItemsSettings : ModSettings
{
    private readonly bool enabled = true;

    public bool IsConfigured => enabled;

    public static void DoWindowContents(Rect inRect)
    {
        Widgets.Label(inRect, "No settings yet!");
    }

}
