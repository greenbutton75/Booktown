using System.Threading.Tasks;
using Basket.Repositories;
using BasketProto;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Basket.Services
{
    public class GrpcBasketService : GrpcBasket.GrpcBasketBase
    {
        private readonly ILogger _logger;
        private readonly IBasketRepository _basketRepository;

        public GrpcBasketService(ILoggerFactory loggerFactory, IBasketRepository basketRepository)
        {
            _logger = loggerFactory.CreateLogger<GrpcBasketService>();
            _basketRepository = basketRepository;
        }

        public override async Task<GetBasketReply> GetBasket(GetBasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"GetBasket {request.Email}");
            var res = new GetBasketReply { Basket =await _basketRepository.GetBasketAsync(request.Email) };

            return res;
        }
        public override async Task<SetBasketReply> SetBasket(SetBasketRequest request, ServerCallContext context)
        {
            var res = new SetBasketReply { Basket = await _basketRepository.SetBasketAsync(request.Basket) };

            return res;
        }
        public override async Task<DeleteBasketReply> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
        {
            var res = new DeleteBasketReply { Basket = await _basketRepository.DeleteBasketAsync(request.Email) };

            return res;
        }

    }
}