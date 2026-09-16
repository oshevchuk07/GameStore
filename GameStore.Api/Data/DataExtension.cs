using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
  public static void MigrateDb(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

    dbContext.Database.Migrate();
  }

  public static void AddGameStoreDb(this WebApplicationBuilder builder)
  {
    var connString = builder.Configuration.GetConnectionString("GameStore");
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

  }
}