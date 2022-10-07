using System.Reflection;
using Akatsuki.DifficultyCalculator.Models;
using Akatsuki.DifficultyCalculator.Requests;
using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;

namespace Akatsuki.DifficultyCalculator.Services;

public class OsuService
{
    private readonly List<Ruleset> _availableRulesets = GetRulesets();
    private readonly Dictionary<DifficultyRequest, DifficultyAttributes> _difficultyMap = new();

    public async Task<DifficultyAttributes?> GetDifficultyAttributes(DifficultyRequest request)
    {
        if (_difficultyMap.TryGetValue(request, out var attributes))
            return attributes;

        var ruleset = _availableRulesets.First(r => r.RulesetInfo.OnlineID == request.RulesetId);
        var apiMods = request.GetMods(ruleset);
        var mods = apiMods.Select(m => m.ToMod(ruleset)).ToArray();

        var beatmap = await GetBeatmap(request.BeatmapId);
        if (beatmap == null)
            return null;

        var difficultyCalculator = ruleset.CreateDifficultyCalculator(beatmap);
        var difficultyAttributes = difficultyCalculator.Calculate(mods);

        // json serializer can't work with this for some reason :(
        difficultyAttributes.Mods = Array.Empty<Mod>();
        
        _difficultyMap[request] = difficultyAttributes;
        return difficultyAttributes;
    }

    private static async Task<WorkingBeatmap?> GetBeatmap(int beatmapId)
    {
        var req = new WebRequest($"https://old.ppy.sh/osu/{beatmapId}");

        await req.PerformAsync();
        if (req.ResponseStream.Length == 0)
            return null;

        var responseBuffer = req.GetResponseData();
        return new LoaderWorkingBeatmap(new MemoryStream(responseBuffer));
    }

    private static List<Ruleset> GetRulesets()
    {
        const string rulesetLibraryPrefix = "osu.Game.Rulesets";

        var rulesetsToProcess = new List<Ruleset>();

        foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{rulesetLibraryPrefix}.*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                var type = assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(Ruleset)));
                rulesetsToProcess.Add((Ruleset)Activator.CreateInstance(type)!);
            }
            catch
            {
                throw new Exception($"Failed to load ruleset ({file})");
            }
        }

        return rulesetsToProcess;
    }
}