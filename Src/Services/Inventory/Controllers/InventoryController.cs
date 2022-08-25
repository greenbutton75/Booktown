using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Queries;
using Inventory.Commands;
using Inventory.Dtos;
using Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("accounts/v1")]
    [Produces("application/json")]
    public class InventoryController : ControllerBase
    {

        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public InventoryController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }


        [HttpPost("Load")]
        public async Task<IActionResult> Load([FromBody] IReadOnlyList<InventoryItemDto> loadItems, CancellationToken cancellationToken)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            foreach (var loadItem in loadItems)
            {
                items.Add(Mapping.Map<InventoryItemDto, InventoryItem>(loadItem));
            }

            var command = new LoadCommand.Command(items);

            await _commandBus.SendAsync(command, cancellationToken);

            return Ok();
        }

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
        /*
        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllQuery.Result>> Get(CancellationToken cancellationToken)
        {
            var query = new GetAllQuery.Query();

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }
        */
       
    }
}