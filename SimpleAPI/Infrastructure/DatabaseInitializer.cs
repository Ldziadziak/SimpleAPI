using Microsoft.EntityFrameworkCore;
using SimpleAPI.DbContexts;

namespace SimpleAPI.Infrastructure;
public static class DatabaseInitializer
{
  public static async Task MigrateDatabaseAsync(IServiceProvider services)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();

    await using var db = services.GetRequiredService<CustomerContext>();

    try
    {
      await db.Database.MigrateAsync();
      logger.LogInformation("Database successfully migrated.");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Database migration failed.");
    }
  }
}