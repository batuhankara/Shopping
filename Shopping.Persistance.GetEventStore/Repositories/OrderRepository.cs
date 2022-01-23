using Shopping.Domain.OrderAggregate;
using Shopping.Infra.Common.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Persistance.GetEventStore.Repositories
{
    public interface IOrderRepository : IEventSourcedRepository<OrderAggregate>
    {

    }
    public class OrderRepository : EventStoreRepository<OrderAggregate>, IOrderRepository
    {
        public OrderRepository(IEventStoreConnectionManager eventStoreConnectionManager) : base(eventStoreConnectionManager)
        {
        }
    }
}
