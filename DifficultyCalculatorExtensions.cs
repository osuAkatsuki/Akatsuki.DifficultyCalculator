using Akatsuki.DifficultyCalculator.Requests;
using MediatR;

namespace Akatsuki.DifficultyCalculator;

public static class DifficultyCalculatorExtensions
{
    public static void MediateGet<TRequest>(this WebApplication app, string template) where TRequest : IHttpRequest
    {
        app.MapGet(template, async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
    }
}