using Ratings.Models;

namespace Ratings.Repositories;

public interface IRatingsRepository
{
    Task<List<RatingItem>> GetReviewsForItemAsync(string ProductId, int Rating);
}

