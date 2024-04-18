using Microsoft.EntityFrameworkCore;

namespace BGM.Test.Web.Helpers;

public static class MigrationHelper
{
    public static WebApplication MigrateDatabase<T>(this WebApplication applicationBuilder) where T : DbContext
    {
        using IServiceScope? scope = applicationBuilder.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<T>();
        db.Database.Migrate();

        return applicationBuilder;
    }
}