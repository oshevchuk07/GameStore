using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetName";
  private static readonly List<GameDto> games = [
    new (1, "Street fighter", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
    new (2, "Final Fantasy", "RPG", 99.99M, new DateOnly(2024, 1, 10)),
    new (1, "Astro Bot", "Platformer", 59.99M, new DateOnly(2026, 10, 25)),
  ];

  public static void MapGamesEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/games");

    // GET all available games
    group.MapGet("/", () => games);

    group.MapGet("/{id}", (int id) =>
    {
      var item = games.Find(el => el.Id == id);
      if (item is null)
      {
        return Results.NotFound(new { message = $"Game {id} not found" });
      }
      return Results.Ok(item);
    }).WithName(GetGameEndpointName);

    /*
    * POST
    */
    group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
    {
      // Game game = new()
      // {
      //   Name = newGame.Name

      // };

      GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
      );

      games.Add(game);

      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
    });

    group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
    {
      var index = games.FindIndex(el => el.Id == id);
      if (index >= 0)
      {
        games[index] = games[index] with
        {
          Name = updatedGame.Name,
          Genre = updatedGame.Genre,
          Price = updatedGame.Price
        };
      }

      return Results.NoContent();
    });

    group.MapDelete("/{id}", (int id) =>
    {
      var index = games.FindIndex(el => el.Id == id);
      if (index >= 0)
      {
        games.RemoveAt(index);
      }

      return Results.NoContent();
    });

  }
}