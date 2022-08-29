using Ratings.Models;

namespace Ratings.Repositories;

public interface IRatingsRepository
{
    Task<List<RatingItem>> GetRatingsForItemAsync(string ProductId, int Rating);
}

