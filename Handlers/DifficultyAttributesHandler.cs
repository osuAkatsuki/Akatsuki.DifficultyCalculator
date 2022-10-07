using Akatsuki.DifficultyCalculator.Requests;
using Akatsuki.DifficultyCalculator.Services;
using MediatR;

namespace Akatsuki.DifficultyCalculator.Handlers;

public class DifficultyAttributesHandler : IRequestHandler<DifficultyRequest, IResult>
{
    private readonly OsuService _osuService;

    public DifficultyAttributesHandler(OsuService osuService)
    {
        _osuService = osuService;
    }

    public async Task<IResult> Handle(DifficultyRequest request, CancellationToken cancellationToken)
    {
        var attributes = await _osuService.GetDifficultyAttributes(request);
        if (attributes == null)
            return Results.NotFound(new
            {
                message = "Couldn't calculate attributes for this beatmap"
            });

        return Results.Ok(attributes);
    }
}