namespace Ratings.Models
{
    public class RatingItem
    {
        public string ProductId { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public DateTime Created { get; set; }
        public string Review { get; set; }
    }
}