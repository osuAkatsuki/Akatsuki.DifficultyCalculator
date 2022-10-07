using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using osu.Game.Skinning;

namespace Akatsuki.DifficultyCalculator.Models;

public class LoaderWorkingBeatmap : WorkingBeatmap
{
    private readonly Beatmap _beatmap;

    public LoaderWorkingBeatmap(Stream stream)
        : this(new LineBufferedReader(stream))
    {
        stream.Dispose();
    }

    private LoaderWorkingBeatmap(LineBufferedReader reader)
        : this(Decoder.GetDecoder<Beatmap>(reader).Decode(reader))
    {
    }

    private LoaderWorkingBeatmap(Beatmap beatmap)
        : base(beatmap.BeatmapInfo, null)
    {
        _beatmap = beatmap;

        beatmap.BeatmapInfo.Ruleset = beatmap.BeatmapInfo.Ruleset.OnlineID switch
        {
            0 => new OsuRuleset().RulesetInfo,
            1 => new TaikoRuleset().RulesetInfo,
            2 => new CatchRuleset().RulesetInfo,
            3 => new ManiaRuleset().RulesetInfo,
            _ => beatmap.BeatmapInfo.Ruleset
        };
    }

    protected override IBeatmap GetBeatmap() => _beatmap;
    protected override Texture GetBackground() => null!;
    protected override Track GetBeatmapTrack() => null!;
    protected override ISkin GetSkin() => null!;
    public override Stream GetStream(string storagePath) => null!;
}