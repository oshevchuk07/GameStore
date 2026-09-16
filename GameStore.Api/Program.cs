
using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using GameStore.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var connString = "Data Source=GameStore.db";
builder.Services.AddSqlite<GameStoreContext>(
  connString,
  optionsAction: options => options.UseSeeding((context, _) =>
  {
    if (!context.Set<Game>().Any())
    {
      context.Set<Game>().AddRange(
        new Game { Genre = "Test", Name = "Test", Price = 12, ReleaseDate = new DateOnly(2026, 2, 6) }
      );

      context.SaveChanges();
    }
  })
  );

var app = builder.Build();

app.MapGamesEndpoints();

app.MigrateDb();

app.Run();
