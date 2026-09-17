using System.Timers;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
  public static void MapGenresEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/genres");

    group.MapGet("/", async (GameStoreContext dbContext) =>
    {
      var items = await dbContext.Genres.Select(genre => new GenreDto(genre.Id, genre.Name)).AsNoTracking().ToListAsync();
      return Results.Ok(items);
    });

    group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
    {
      var item = await dbContext.Genres.FindAsync(id);

      if (item is null)
      {
        return Results.NotFound(new { message = "not found" });
      }

      return Results.Ok(item);
    });

   
  }
}