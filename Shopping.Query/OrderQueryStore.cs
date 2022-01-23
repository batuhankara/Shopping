using Shopping.Persistance.ReadStore;
using Shopping.Persistance.ReadStore.DbContext;
using Microsoft.EntityFrameworkCore;
namespace Shopping.Query
{
    public interface IOrderQueryStore
    {
        Task<OrderDetailView> GetOrderByProductsWithId(Guid guid);
    }
    public class OrderQueryStore : IOrderQueryStore
    {
        private readonly OrderDbContext orderDbContext;

        public OrderQueryStore(OrderDbContext orderDbContext)
        {
            this.orderDbContext = orderDbContext;
        }

        public async Task<OrderDetailView> GetOrderByProductsWithId(Guid guid)
        {
            return await orderDbContext.OrderDetailView.Include(x=>x.OrderItemViews).FirstOrDefaultAsync(x => x.Id == guid);

        }
    }
}
