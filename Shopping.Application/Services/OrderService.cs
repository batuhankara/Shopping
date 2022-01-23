using Shopping.Common.Messaging.Order.Commands;
using Shopping.Domain.OrderAggregate;
using Shopping.Infra.Common;
using Shopping.Persistance.GetEventStore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Application.Services
{
    public interface IOrderCommandService
    {
        Task<IEnumerable<IEvent>> Handle(RemoveOrderItemCommand command);
        Task<IEnumerable<IEvent>> Handle(CreateOrderCommand command);
        Task<IEnumerable<IEvent>> Handle(AddProductCommand command);

    }
    public class OrderCommandService : IOrderCommandService
    {
        private readonly IOrderRepository orderRepository;

        public OrderCommandService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<IEnumerable<IEvent>> Handle(CreateOrderCommand command)
        {
            if (command.AddProductToOrderCommands!=null &&command.AddProductToOrderCommands.Count>5)
            {
                throw new Exception("5 or more products");
            }
            var existingOrder = await orderRepository.GetById(command.OrderId);

            if (existingOrder != null)
            {
                if (existingOrder.HasPreviouslyExecutedCommand(command))
                {
                    return existingOrder.GetEventsProducedInResponseToCommand(command);
                }

                throw new Exception("already exists");
            }
            OrderAggregate order = new OrderAggregate();
            try
            {

                order.Handle<CreateOrderCommand>(command);

                return await orderRepository.Save(order, command);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<IEvent>> Handle(RemoveOrderItemCommand command)
        {
            var existingOrder = await orderRepository.GetById(command.OrderId);

            if (existingOrder != null)
            {
                if (existingOrder.HasPreviouslyExecutedCommand(command))
                {
                    return existingOrder.GetEventsProducedInResponseToCommand(command);
                }
                existingOrder.Handle(command);
                return await orderRepository.Save(existingOrder, command);

            }
            return null;
        }

        public async Task<IEnumerable<IEvent>> Handle(AddProductCommand command)
        {
            var existingOrder = await orderRepository.GetById(command.OrderId);

            if (existingOrder != null)
            {
                if (existingOrder.HasPreviouslyExecutedCommand(command))
                {
                    return existingOrder.GetEventsProducedInResponseToCommand(command);
                }
                existingOrder.Handle(command);
                return await orderRepository.Save(existingOrder, command);

            }
            return null;
        }
    }
}
