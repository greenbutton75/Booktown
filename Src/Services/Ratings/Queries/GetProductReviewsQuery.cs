using FluentValidation;
using Infrastructure.Core.Queries;
using Ratings.Models;
using Ratings.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ratings.Queries
{
    public class GetProductReviewsQuery
    {
        public class Query : IQuery<Result>
        {
            public string ProductId { get; set; }
            public int Rating { get; set; }
        }

        public class Result
        {
            public ICollection<RatingItem> RatingItems { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.ProductId).NotEmpty();
            }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            private readonly IRatingsRepository _repository;

            public Handler(IRatingsRepository repository)
            {
                _repository = repository;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var ratings = await _repository.GetReviewsForItemAsync(query.ProductId, query.Rating);

                var result = new Result
                {
                    RatingItems = ratings
                };

                return result;
            }

        }
    }
}
