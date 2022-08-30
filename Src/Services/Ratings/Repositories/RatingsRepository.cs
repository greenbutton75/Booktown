using Infrastructure.Core.Exceptions;
using Infrastructure.Core.Extensions;
using Neo4j.Driver;
using Ratings.Models;

namespace Ratings.Repositories;

public class RatingsRepository : IRatingsRepository
{
    private readonly ILogger<RatingsRepository> _logger;
    private readonly IDriver _driver;

    public RatingsRepository(ILoggerFactory loggerFactory, IDriver driver)
    {
        _logger = loggerFactory.CreateLogger<RatingsRepository>();
        _driver = driver;
    }

    // _session.WriteTransaction(tx => tx.Run("CREATE (a:Person {name: $name})", new {name}).Consume());

    public async Task<List<RatingItem>> GetReviewsForItemAsync(string ProductId, int Rating)
    {
        var session = GetSession();
        try
        {
            IResultCursor cursor = await session.RunAsync($"MATCH (a:Person) WHERE a.name = $ProductId RETURN a.name as name, a.born as born", new { ProductId = ProductId });
            List<RatingItem> people = await cursor.ToListAsync(record => new RatingItem { ProductId = record["name"].As<string>(), Rating = record["born"].As<int>() });

            return people;
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    private IAsyncSession GetSession()
    { 
        return _driver.AsyncSession(o => o.WithDatabase("neo4j"));
    }

}
