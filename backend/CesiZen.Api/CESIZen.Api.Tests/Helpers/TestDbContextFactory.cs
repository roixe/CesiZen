using CesiZen.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Tests.Helpers;

public static class TestDbContextFactory
{
    public static CesiZenDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CesiZenDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CesiZenDbContext(options);
    }
}