using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CesiZen.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CesiZenDbContext>
{
    public CesiZenDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CesiZenDbContext>()
            .UseMySql(
                "Server=design;Database=design;User=design;Password=design;",
                new MySqlServerVersion(new Version(8, 0, 0))  
            )
            .Options;

        return new CesiZenDbContext(options);
    }
}