using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Queries;
using Ratings.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratings.Queries;

namespace Ratings.Controllers
{
    [ApiController]
    [Route("accounts/v1")]
    [Produces("application/json")]
    public class RatingsController : ControllerBase
    {

        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public RatingsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        /*
        [HttpPost("Data")]
        public async Task<IActionResult> Load([FromBody] string ProductID, int Rating, CancellationToken cancellationToken)
        {
            List<RatingItem> items = new List<InventoryItem>();
            foreach (var loadItem in loadItems)
            {
                items.Add(Mapping.Map<InventoryItemDto, InventoryItem>(loadItem));
            }

            var command = new LoadCommand.Command(items);

            await _commandBus.SendAsync(command, cancellationToken);

            return Ok();
        }
        */
        /* This is bus event
        [HttpGet("Spend/{id}/{quantity}")]
        public async Task<IActionResult> Spend(string id, int quantity, CancellationToken cancellationToken)
        {
            var command = new SpendCommand.Command
            (
                new InventoryItem {
                ProductId = id,
                Quantity = quantity
                }
            );

            await _commandBus.Send(command, cancellationToken);

            return Ok();
        }
        */
        
        [HttpGet("GetAll/{ProductID}/{Rating}")]
        public async Task<ActionResult<GetProductRatingsQuery.Result>> Get(string ProductID, int Rating, CancellationToken cancellationToken)
        {
            var query = new GetProductRatingsQuery.Query
            {
                ProductId = ProductID,
                Rating = Rating
            };

            var result = await _queryBus.SendAsync(query, cancellationToken);

            return Ok(result);
        }
        
       
    }
}