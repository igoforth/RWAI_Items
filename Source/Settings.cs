namespace AIItems;

public partial class AIItemsSettings : ModSettings
{
    public bool enabled = true;

    // reporting settings

    // Power Insight settings
    public bool reportEnergyStatus = true;
    public int reportEnergyFrequency = 8000;
    public bool reportEnergyImmediate = false;

    // Research Insight settings
    public bool reportResearchStatus = true;
    public int reportResearchFrequency = 60000;
    public bool reportResearchImmediate = false;

    // Thoughts & Mood Insight settings
    public bool reportColonistThoughts = true;
    public int reportColonistThoughtsFrequency = 60000;
    public bool reportColonistThoughtsImmediate = false;

    // Interpersonal Insight settings
    public bool reportColonistOpinions = false; // Initially disabled
    public int reportColonistOpinionsFrequency = 60000;
    public bool reportColonistOpinionsImmediate = false;

    // Detailed Colonist Insight settings
    public bool reportColonistRoster = false; // Initially disabled
    public int reportColonistRosterFrequency = 60000;
    public bool reportColonistRosterImmediate = false;

    // Rooms Insight settings
    public bool reportRoomStatus = false; // Initially disabled
    public int reportRoomStatusFrequency = 60000;
    public bool reportRoomStatusImmediate = false;

    // ------------------------------------

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref enabled, "enabled", true);

        // Thoughts & Mood Insight settings
        Scribe_Values.Look(
            ref reportColonistThoughts,
            "reportColonistThoughts",
            defaultValue: true
        );
        Scribe_Values.Look(
            ref reportColonistThoughtsFrequency,
            "reportColonistThoughtsFrequency",
            defaultValue: 60000
        );
        Scribe_Values.Look(
            ref reportColonistThoughtsImmediate,
            "reportColonistThoughtsImmediate",
            defaultValue: false
        );

        // Interpersonal Insight settings
        Scribe_Values.Look(
            ref reportColonistOpinions,
            "reportColonistOpinions",
            defaultValue: true
        );
        Scribe_Values.Look(
            ref reportColonistOpinionsFrequency,
            "reportColonistOpinionsFrequency",
            defaultValue: 60000
        );
        Scribe_Values.Look(
            ref reportColonistOpinionsImmediate,
            "reportColonistOpinionsImmediate",
            defaultValue: false
        );

        // Power Insight settings
        Scribe_Values.Look(ref reportEnergyStatus, "reportEnergyStatus", defaultValue: false);
        Scribe_Values.Look(ref reportEnergyFrequency, "reportEnergyFrequency", defaultValue: 8000);
        Scribe_Values.Look(ref reportEnergyImmediate, "reportEnergyImmediate", defaultValue: false);

        // Research Insight settings
        Scribe_Values.Look(ref reportResearchStatus, "reportResearchStatus", defaultValue: false);
        Scribe_Values.Look(
            ref reportResearchFrequency,
            "reportResearchFrequency",
            defaultValue: 60000
        );
        Scribe_Values.Look(
            ref reportResearchImmediate,
            "reportResearchImmediate",
            defaultValue: false
        );

        // Detailed Colonist Insight settings
        Scribe_Values.Look(ref reportColonistRoster, "reportColonistRoster", defaultValue: false);
        Scribe_Values.Look(
            ref reportColonistRosterFrequency,
            "reportColonistRosterFrequency",
            defaultValue: 60000
        );
        Scribe_Values.Look(
            ref reportColonistRosterImmediate,
            "reportColonistRosterImmediate",
            defaultValue: false
        );

        // Rooms Insight settings
        Scribe_Values.Look(ref reportRoomStatus, "reportRoomStatus", defaultValue: false);
        Scribe_Values.Look(
            ref reportRoomStatusFrequency,
            "reportRoomStatusFrequency",
            defaultValue: 60000
        );
        Scribe_Values.Look(
            ref reportRoomStatusImmediate,
            "reportRoomStatusImmediate",
            defaultValue: false
        );

        // if (Scribe.mode == LoadSaveMode.PostLoadInit)
        // {
        //     if (azureVoice != null && personas.NullOrEmpty())
        //     {
        //         //ChatGPTModelPrimary = Tools.chatGPTModels[0];
        //         personas ??= [];
        //         personas.Add(
        //             new Persona()
        //             {
        //                 name = "Default",
        //                 azureVoiceLanguage = azureVoiceLanguage,
        //                 azureVoice = azureVoice,
        //                 azureVoiceStyle = azureVoiceStyle,
        //                 azureVoiceStyleDegree = azureVoiceStyleDegree,
        //                 speechRate = speechRate,
        //                 speechPitch = speechPitch,
        //                 phrasesLimit = phrasesLimit,
        //                 phraseBatchSize = phraseBatchSize,
        //                 phraseDelayMin = phraseDelayMin,
        //                 phraseDelayMax = phraseDelayMax,
        //                 phraseMaxWordCount = phraseMaxWordCount,
        //                 historyMaxWordCount = historyMaxWordCount,
        //                 personality = personality,
        //                 personalityLanguage = personalityLanguage
        //             }
        //         );
        //     }
        // }
    }

    public bool IsConfigured => enabled;

    public Vector2 scrollPosition = Vector2.zero;

    public static int selectedIndex = -1;
    public static readonly Color listBackground = new(32 / 255f, 36 / 255f, 40 / 255f);
    public static readonly Color highlightedBackground = new(74 / 255f, 74 / 255f, 74 / 255f, 0.5f);
    public static readonly Color background = new(74 / 255f, 74 / 255f, 74 / 255f);

    // Active state colors
    public static readonly Color listBackgroundActive = new(16 / 255f, 72 / 255f, 35 / 255f);
    public static readonly Color highlightedBackgroundActive =
        new(37 / 255f, 144 / 255f, 53 / 255f, 0.5f);
    public static readonly Color backgroundActive = new(37 / 255f, 144 / 255f, 53 / 255f);

    public void DoWindowContents(Rect inRect)
    {
        // Rect rect;

        // var spacing = 20f; // Alternatively adjust the spacing if needed
        // var totalSpaceBetweenColumns = spacing * 2;

        // // New calculation for column widths
        // var slimColumnFactor = 0.5f; // Middle column is 50% of the others
        // var availableSpace = inRect.width - totalSpaceBetweenColumns; // Total available width minus the spacing
        // var normalColumnWidth = (availableSpace / (2 + slimColumnFactor)); // Width for the 1st and 3rd columns
        // var middleColumnWidth = normalColumnWidth * slimColumnFactor; // Width for the middle (slimmer) column

        // var list = new Listing_Standard { ColumnWidth = normalColumnWidth };
        // list.Begin(inRect);

        // list.Gap(16f);
        // if (Widgets.ButtonText(list.GetRect(UX.ButtonHeight), "AI Configuration"))
        //     Find.WindowStack.Add(new SettingsLLM(this));

        // // for three columns with 20px spacing
        // var width = normalColumnWidth;

        // var activeConfig = userApiConfigs?.FirstOrDefault(a => a.Active);
        // var activeModelId =
        //     (activeConfig?.ModelId.Length ?? 0) == 0
        //         ? "None"
        //         : activeConfig.ModelId.Split('/').Last() ?? activeConfig.ModelId;
        // var activeSecondaryId =
        //     (activeConfig?.SecondaryModelId.Length ?? 0) == 0
        //         ? "None"
        //         : activeConfig.SecondaryModelId.Split('/').Last() ?? activeConfig.SecondaryModelId;

        // list.Label(
        //     "00FF22",
        //     "Active Provider:",
        //     activeConfig?.Provider ?? "None",
        //     activeConfig == null
        //         ? "To active a provider, click on the AI Configuration button above."
        //         : "This is the active provider you have selected in the AI Configuration.",
        //     0
        // );

        // list.Label(
        //     "FFFFFF",
        //     "Active Model:",
        //     activeModelId,
        //     activeConfig == null
        //         ? "To active a provider, click on the AI Configuration button above."
        //         : "This is the the active model you have selected in the AI Configuration.",
        //     0
        // );

        // list.Label(
        //     "FFFFFF",
        //     "Secondary Model:",
        //     activeSecondaryId,
        //     activeConfig == null
        //         ? "To active a provider, click on the AI Configuration button above."
        //         : activeSecondaryId.Length == 0
        //             ? "To select a secondary model, you must check the 'Alternate between two models' checkbox in the AI Configuration and type or select a model id."
        //             : "This is the active secondary model you have selected in the AI Configuration.",
        //     0
        // );

        // list.Gap(10f);

        // foreach (
        //     var config in userApiConfigs
        //         .Where(a => a.Provider.Length > 0)
        //         .OrderByDescending(b => b.Active)
        // )
        // {
        //     list.Label(
        //         config.Active ? "00FF22" : "FFFF00",
        //         $"{config.Provider}",
        //         $"Chars Sent: {Tools.FormatNumber(config.CharactersSent)} | Received: {Tools.FormatNumber(config.CharactersReceived)}",
        //         $"Total characters sent and received while using {config.Provider}.",
        //         0
        //     );
        // }
        // list.Gap(10f);

        // list.Label(
        //     "FFFF00",
        //     "Azure - Speech Services",
        //     $"Chars Sent: {Tools.FormatNumber(charactersSentAzure)}",
        //     "Total characters sent while using Azure Speech Services."
        // );
        // var prevRegion = azureSpeechRegion;
        // list.TextField(ref azureSpeechRegion, "Region");
        // if (azureSpeechRegion != prevRegion)
        //     Personas.UpdateVoiceInformation();
        // list.Gap(6f);
        // var prevKey = azureSpeechKey;
        // list.TextField(ref azureSpeechKey, "API Key (paste only)", true, () => azureSpeechKey = "");
        // if (
        //     azureSpeechKey != ""
        //     && azureSpeechKey != prevKey
        //     && azureSpeechRegion.NullOrEmpty() == false
        // )
        //     TTS.TestKey(new Persona(), () => Personas.UpdateVoiceInformation());

        // list.Gap(16f);

        // list.Label("FFFF00", "Miscellaneous");
        // list.Slider(
        //     ref speechVolume,
        //     0f,
        //     10f,
        //     f => $"Speech volume: {f.ToPercentage(false)}",
        //     0.01f
        // );
        // list.CheckboxLabeled("Show speech as subtitles", ref showAsText);
        // list.Gap(6f);
        // rect = list.GetRect(UX.ButtonHeight);
        // if (Widgets.ButtonText(rect, "Reset Stats"))
        // {
        //     charactersSentOpenAI = 0;
        //     charactersSentAzure = 0;
        // }

        // list.NewColumn();
        // //----------------------------- Middle Column ----------------------------------------//
        // list.ColumnWidth = middleColumnWidth;

        // list.Gap(16f);
        // if (Widgets.ButtonText(list.GetRect(UX.ButtonHeight), "AI Insights"))
        //     Find.WindowStack.Add(new DetailedReportSettingsWindow(this));

        // list.Gap(10f);
        // list.Label("FFFF00", "Active personas", "", "All these personas are active.");

        // var height = inRect.height - UX.ButtonHeight - 24f - list.CurHeight;
        // var outerRect = list.GetRect(height);
        // var listHeight = height;
        // var innerRect = new Rect(0f, 0f, list.ColumnWidth, listHeight);

        // Widgets.DrawBoxSolid(outerRect, listBackground);
        // Widgets.BeginScrollView(outerRect, ref scrollPosition, innerRect, true);

        // var list2 = new Listing_Standard();
        // list2.Begin(innerRect);
        // var rowHeight = 24;
        // var i = 0;
        // var y = 0f;
        // foreach (var persona in personas)
        // {
        //     PersonaRow(new Rect(0, y, innerRect.width, rowHeight), persona, i++);
        //     y += rowHeight;
        // }
        // list2.End();

        // Widgets.EndScrollView();

        // var bRect = list.GetRect(24);
        // if (Widgets.ButtonImage(bRect.LeftPartPixels(24), Graphics.ButtonAdd[1]))
        // {
        //     Event.current.Use();
        //     selected = new Persona();
        //     personas.Add(selected);
        //     selectedIndex = personas.IndexOf(selected);
        // }
        // bRect.x += 32;
        // if (
        //     Widgets.ButtonImage(
        //         bRect.LeftPartPixels(24),
        //         Graphics.ButtonDel[selected != null ? 1 : 0]
        //     )
        // )
        // {
        //     Event.current.Use();
        //     _ = personas.Remove(selected);
        //     var newCount = personas.Count;
        //     if (newCount == 0)
        //     {
        //         selectedIndex = -1;
        //         selected = null;
        //     }
        //     else
        //     {
        //         while (newCount > 0 && selectedIndex >= newCount)
        //             selectedIndex--;
        //         selected = personas[selectedIndex];
        //     }
        // }
        // bRect.x += 32;
        // var dupable = selected != null;
        // if (
        //     Widgets.ButtonImage(bRect.LeftPartPixels(24), Graphics.ButtonDup[dupable ? 1 : 0])
        //     && dupable
        // )
        // {
        //     Event.current.Use();
        //     var namePrefix = Regex.Replace(selected.name, @" \d+$", "");
        //     var existingNames = personas.Select(p => p.name).ToHashSet();
        //     for (var n = 1; true; n++)
        //     {
        //         var newName = $"{namePrefix} {n}";
        //         if (existingNames.Contains(newName) == false)
        //         {
        //             var xml = selected.ToXml();
        //             selected = new Persona();
        //             Persona.PersonalityFromXML(xml, selected);
        //             selected.name = newName;
        //             personas.Add(selected);
        //             selectedIndex = personas.IndexOf(selected);
        //             break;
        //         }
        //     }
        // }

        // list.NewColumn();
        // //----------------------------- Right Column ----------------------------------------//
        // list.ColumnWidth = normalColumnWidth;

        // if (selected != null)
        // {
        //     var curY = list.curY;
        //     _ = list.Label("Persona Name");
        //     list.curY = curY;
        //     var cw = list.ColumnWidth / 3f;
        //     list.curX += cw;
        //     list.ColumnWidth -= cw;
        //     selected.name = list.TextEntry(selected.name);
        //     list.curX -= cw;
        //     list.ColumnWidth += cw;
        //     list.Gap(16f);

        //     var buttonWidth = (rect.width - 40) / 3; // Spacing between buttons is 20, hence 40 for two spaces.

        //     list.Languages(
        //         LanguageDatabase.AllLoadedLanguages,
        //         selected.azureVoiceLanguage,
        //         l => l.DisplayName,
        //         l =>
        //         {
        //             selected.azureVoiceLanguage = l == null ? "-" : l.FriendlyNameEnglish;
        //             Personas.UpdateVoiceInformation();
        //         },
        //         buttonWidth,
        //         0
        //     );
        //     list.Voices(selected, buttonWidth, 1);
        //     if (UX.HasVoiceStyles(selected))
        //         list.VoiceStyles(selected, buttonWidth, 2);
        //     list.Gap(30f);

        //     list.Gap(16f);

        //     list.Slider(
        //         ref selected.azureVoiceStyleDegree,
        //         0f,
        //         2f,
        //         f => $"Style degree: {f.ToPercentage(false)}",
        //         0.01f
        //     );
        //     list.Slider(
        //         ref selected.speechRate,
        //         -0.5f,
        //         0.5f,
        //         f => $"Speech rate: {f.ToPercentage()}",
        //         0.01f
        //     );
        //     list.Slider(
        //         ref selected.speechPitch,
        //         -0.5f,
        //         0.5f,
        //         f => $"Speech pitch: {f.ToPercentage()}",
        //         0.01f
        //     );

        //     list.Gap(16f);

        //     rect = list.GetRect(UX.ButtonHeight);
        //     var buttonRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);

        //     if (Widgets.ButtonText(buttonRect, "Personality"))
        //         Dialog_Personality.Show(selected);

        //     buttonRect.x += buttonWidth + 20; // Move x position by button width plus spacing

        //     if (
        //         Widgets.ButtonText(
        //             buttonRect,
        //             selected.personalityLanguage == "-" ? "Language" : selected.personalityLanguage
        //         )
        //     )
        //         UX.LanguageChoiceMenu(
        //             Tools.commonLanguages,
        //             l => l,
        //             l => selected.personalityLanguage = l ?? "-"
        //         );

        //     buttonRect.x += buttonWidth + 20; // Move x position by button width plus spacing

        //     if (Widgets.ButtonText(buttonRect, "Test"))
        //         TTS.TestKey(selected, null);

        //     list.Gap(16f);

        //     _ = list.Label(
        //         "Sending game information",
        //         -1,
        //         "RimGPT limits when and what it sends to ChatGPT. It collects phrases from the game and other personas until after some time it sends some of the phrases batched together to create a comment."
        //     );
        //     list.Slider(
        //         ref selected.phrasesLimit,
        //         1,
        //         100,
        //         n => $"Max phrases: {n}",
        //         1,
        //         "How many unsent phrases should RimGPT keep at a maximum?"
        //     );
        //     selected.phraseBatchSize = Mathf.Min(selected.phraseBatchSize, selected.phrasesLimit);
        //     list.Slider(
        //         ref selected.phraseBatchSize,
        //         1,
        //         selected.phrasesLimit,
        //         n => $"Batch size: {n} phrases",
        //         1,
        //         "How many phrases should RimGPT send batched together in its data to ChatGPT?"
        //     );
        //     list.Gap(16f);
        //     _ = list.Label(
        //         "Delay between comments",
        //         -1,
        //         "To optimize, RimGPT collects text and phrases and only sends it in intervals to ChatGPT to create comments."
        //     );
        //     list.Slider(
        //         ref selected.phraseDelayMin,
        //         5f,
        //         1200f,
        //         f => $"Min: {Mathf.FloorToInt(f + 0.01f)} sec",
        //         1f,
        //         2,
        //         "RimGPT creates comments in intervals. What is the shortest time between comments?"
        //     );
        //     if (selected.phraseDelayMin > selected.phraseDelayMax)
        //         selected.phraseDelayMin = selected.phraseDelayMax;
        //     var oldMax = selected.phraseDelayMax;
        //     list.Slider(
        //         ref selected.phraseDelayMax,
        //         5f,
        //         1200f,
        //         f => $"Max: {Mathf.FloorToInt(f + 0.01f)} sec",
        //         1f,
        //         2,
        //         "RimGPT creates comments in intervals. What is the longest time between comments?"
        //     );
        //     if (selected.phraseDelayMax < selected.phraseDelayMin)
        //         selected.phraseDelayMax = selected.phraseDelayMin;
        //     if (oldMax > selected.phraseDelayMax)
        //         selected.nextPhraseTime = DateTime.Now.AddSeconds(selected.phraseDelayMin);
        //     list.Gap(16f);
        //     _ = list.Label("Comments");
        //     list.Slider(
        //         ref selected.phraseMaxWordCount,
        //         1,
        //         160,
        //         n => $"Max words: {n}",
        //         1,
        //         "RimGPT instructs ChatGPT to generate comments that are no longer than this amount of words."
        //     );
        //     list.Gap(16f);
        //     _ = list.Label("History");
        //     list.Slider(
        //         ref selected.historyMaxWordCount,
        //         200,
        //         1200,
        //         n => $"Max words: {n}",
        //         1,
        //         "RimGPT lets ChatGPT create a history summary that is then send together with new requests to form some kind of memory for ChatGPT. What is the maximum size of the history?"
        //     );
        //     list.Gap(16f);
        //     TooltipHandler.TipRegion(
        //         new Rect(list.curX, list.curY, 200f, 24f),
        //         "Less suitable for personas that speak frequently, and more suitable for a role whom chimes in on more rare occasions. The personality will still need to be customized to fully take advantage of this feature."
        //     );
        //     bool chroniclerToggle = selected.isChronicler;
        //     list.CheckboxLabeled(
        //         "Is Chronicler",
        //         ref chroniclerToggle,
        //         "Enable if this persona should be treated as a Chronicler.  Otherwise they'll receive just the most recent phrases no other persona has received."
        //     );
        //     selected.isChronicler = chroniclerToggle; // Update the persona's isChronicler flag
        //     list.Gap(16f);
        //     width = (list.ColumnWidth - 2 * 20) / 3;
        //     rect = list.GetRect(UX.ButtonHeight);
        //     rect.width = width;
        //     if (Widgets.ButtonText(rect, "Copy"))
        //     {
        //         var share = selected.ToXml();
        //         if (share.NullOrEmpty() == false)
        //             GUIUtility.systemCopyBuffer = share;
        //     }
        //     rect.x += width + 20;
        //     if (Widgets.ButtonText(rect, "Paste"))
        //     {
        //         var text = GUIUtility.systemCopyBuffer;
        //         if (text.NullOrEmpty() == false)
        //             Persona.PersonalityFromXML(text, selected);
        //     }
        //     rect.x += width + 20;
        //     if (Widgets.ButtonText(rect, "Defaults"))
        //     {
        //         var xml = new Persona().ToXml();
        //         Persona.PersonalityFromXML(xml, selected);
        //     }
        // }

        // list.End();
    }

    // public static void PersonaRow(Rect rect, Persona persona, int idx)
    // {
    //     if (persona == selected)
    //         Widgets.DrawBoxSolid(rect, background);
    //     else if (Mouse.IsOver(rect))
    //         Widgets.DrawBoxSolid(rect, highlightedBackground);

    //     var tRect = rect;
    //     tRect.xMin += 3;
    //     tRect.yMin += 1;
    //     _ = Widgets.LabelFit(tRect, persona.name);

    //     if (Widgets.ButtonInvisible(rect))
    //     {
    //         selected = persona;
    //         selectedIndex = idx;
    //     }
    // }
}
