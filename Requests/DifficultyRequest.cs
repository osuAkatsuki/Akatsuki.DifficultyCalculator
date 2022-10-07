using osu.Game.Beatmaps.Legacy;
using osu.Game.Online.API;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch.Mods;
using osu.Game.Rulesets.Mania.Mods;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Taiko.Mods;

namespace Akatsuki.DifficultyCalculator.Requests;

public class DifficultyRequest : IHttpRequest
{
    public int BeatmapId { get; init; }
    public int RulesetId { get; init; }
    public int? Mods { get; init; }

    private int ModsValue => Mods ?? 0;

    public IEnumerable<APIMod> GetMods(Ruleset ruleset)
    {
        var mods = ruleset.ConvertFromLegacyMods((LegacyMods)ModsValue);
        var apiMods = mods.Select(x => new APIMod(x)).OrderBy(m => m.Acronym).ToList();

        apiMods.RemoveAll(m =>
        {
            string acronym = m.Acronym.ToUpper();

            if (string.IsNullOrWhiteSpace(acronym))
                return true;

            switch (acronym)
            {
                case "SCOREV2":
                case "CINEMA":
                case "AUTO":
                    return true;
            }

            return false;
        });

        foreach (var m in apiMods)
        {
            if (m.Acronym == "2P")
                m.Acronym = "DS";
        }

        // for now always add classic mod - akatsuki has no lazer support yet
        Mod classicMod = RulesetId switch
        {
            0 => new OsuModClassic(),
            1 => new TaikoModClassic(),
            2 => new CatchModClassic(),
            3 => new ManiaModClassic(),
            _ => throw new ArgumentException($"Invalid ruleset {RulesetId}")
        };
        apiMods.Add(new APIMod(classicMod));

        return apiMods;
    }
    
    public bool Equals(DifficultyRequest? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return BeatmapId == other.BeatmapId && RulesetId == other.RulesetId && ModsValue == other.ModsValue;
    }
    
    public override bool Equals(object? obj)
        => obj is DifficultyRequest other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        
        hashCode.Add(BeatmapId);
        hashCode.Add(RulesetId);
        hashCode.Add(ModsValue);

        return hashCode.ToHashCode();
    }
}