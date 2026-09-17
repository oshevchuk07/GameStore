using System.ComponentModel;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

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
    group.MapGet("/", async (GameStoreContext dbContext) =>
    {
      var gameData = await dbContext.Games
        .Include(game => game.Genre)
        .Select(game => new GameSummaryDto(game.Id, game.Name, game.Genre!.Name, game.Price, game.ReleaseDate))
        .AsNoTracking()
        .ToListAsync();

      return Results.Ok(gameData);
    });

    group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
    {
      var game = await dbContext.Games.FindAsync(id);

      if (game is null)
      {
        return Results.NotFound(new { message = $"Game {id} not found" });
      }
      return Results.Ok(game);
    }).WithName(GetGameEndpointName);

    /*
    * POST
    */
    group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
    {
      Game game = new()
      {
        Name = newGame.Name,
        GenreId = newGame.GenreId,
        Price = newGame.Price,
        ReleaseDate = newGame.ReleaseDate
      };

      dbContext.Games.Add(game);
      await dbContext.SaveChangesAsync();

      GameDetailsDto gameDto = new(
        game.Id,
        game.Name,
        game.GenreId,
        game.Price,
        game.ReleaseDate
      );

      return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDto.Id }, gameDto);
    });

    group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
    {
      var existingGame = await dbContext.Games.FindAsync(id);

      if (existingGame is null)
      {
        return Results.NotFound();
      }

      existingGame.Name = updatedGame.Name;
      existingGame.GenreId = updatedGame.GenreId;
      existingGame.Price = updatedGame.Price;
      existingGame.ReleaseDate = updatedGame.ReleaseDate;

      await dbContext.SaveChangesAsync();

      return Results.Ok(existingGame);
    });

    group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
    {
      var existingGame = await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

      
      return Results.NoContent();
    });

  }
}