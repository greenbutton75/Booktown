using Infrastructure.Core.Exceptions;
using Infrastructure.Core.Extensions;
using Neo4j.Driver;
using Ratings.Models;

namespace Ratings.Repositories;

public class RatingsRepository : IRatingsRepository
{
    private readonly ILogger<RatingsRepository> _logger;
    private readonly IDriver _driver;
    private readonly IAsyncSession _session;

    public RatingsRepository(ILoggerFactory loggerFactory, IDriver driver)
    {
        _logger = loggerFactory.CreateLogger<RatingsRepository>();
        _driver = driver;
        _session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
    }

    public async Task<List<RatingItem>> GetRatingsForItemAsync(string ProductId, int Rating)
    {
	IResultCursor cursor = await _session.RunAsync($"MATCH (a:Person) WHERE a.name = '{ProductId}' RETURN a.name as name, a.born as born");
	List<RatingItem> people = await cursor.ToListAsync(record => new RatingItem {ProductId = record["name"].As<string>(), Rating = record["born"].As<int>()});


        if (people.Count ==0)
        {
            return null;
        }
        return people;
    }
}
