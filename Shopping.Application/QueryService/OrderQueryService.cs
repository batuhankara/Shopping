using Shopping.Common.Messaging.Order.Dtos;
using Shopping.Query;

namespace Shopping.Application.QueryService
{
    public interface IOrderQueryService
    {
        Task<OrderDetailDto> GetOrderWithProductsById(Guid guid);
    }
    public class OrderQueryService : IOrderQueryService
    {
        private readonly IOrderQueryStore orderQueryStore;

        public OrderQueryService(IOrderQueryStore orderQueryStore)
        {
            this.orderQueryStore = orderQueryStore;
        }

        public async Task<OrderDetailDto> GetOrderWithProductsById(Guid guid)
        {
            var order = await orderQueryStore.GetOrderByProductsWithId(guid);
            if (order == null)
            {
                return null;
            }
            var model = new OrderDetailDto()
            {
                Id = order.Id,
                OrderProductDetailDtos = order.OrderItemViews.Select(x => new OrderProductDetailDto { Price = x.Price, ProductId = x.ProductId }).ToList(),
                Sum = order.Sum,

            };
            return model;
        }
    }
}
