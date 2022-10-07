using Akatsuki.DifficultyCalculator;
using Akatsuki.DifficultyCalculator.Requests;
using Akatsuki.DifficultyCalculator.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(x => x.AsScoped(), typeof(Program));
builder.Services.AddSingleton<OsuService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MediateGet<DifficultyRequest>("attributes");

app.Run();
