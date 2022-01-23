using Shopping.Common.Messaging.Order.Commands;
using Shopping.Common.Messaging.Order.Events;
using Shopping.Domain.OrderAggregate.Entities;
using Shopping.Infra.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Domain.OrderAggregate
{
    public class OrderAggregate : EventSourcedAggregate
    {
        public double Sum { get; private set; }
        public Customer Customer { get; private set; }

        private readonly List<Product> _orderItems;
        public IReadOnlyCollection<Product> OrderItems => _orderItems;

        public OrderAggregate()
        {
            _orderItems = new List<Product>();
        }
        public void Handle(RemoveOrderItemCommand command)
        {
            var deleteItem = this.OrderItems.FirstOrDefault(x => x.Id == command.ProductId);
            this._orderItems.Remove(deleteItem);
            this.Sum = OrderItems.Sum(x => x.Price);
            var @event = new RemovedProductItem(command.OrderId, command.ProductId);
            this.AddToUncommitedEvents(@event);
        }
        public void Handle(AddProductCommand command)
        {
            if (this.OrderItems.Count + 1 > 5)
            {
                throw new Exception("Domain exception you cant have more then 5 item");
            }
            this._orderItems.Add(new Product(command.ProductId, command.ProductName, command.Price));
            this.Sum = this.OrderItems.Sum(w => w.Price);
            var @event = new AddedProductItem(command.OrderId, command.ProductId, command.Price);
            this.AddToUncommitedEvents(@event);
        }
        public void Handle(CreateOrderCommand command)
        {
            this.Id = command.OrderId;
            if (command.AddCustomerCommand != null)
            {
                this.Customer = new Customer(command.AddCustomerCommand.Id, command.AddCustomerCommand.Title);

            }
            if (command.AddProductToOrderCommands != null)
            {
                command.AddProductToOrderCommands.ForEach(x =>
                {
                    var product = new Product(x.Id, x.Name, x.Price);
                    this._orderItems.Add(product);
                });
                this.Sum = OrderItems.Sum(x => x.Price);
            }


            var @event = new OrderCreated(this.Id, this.Sum, this.Customer.Title,
                this.Customer.Id, this.OrderItems.Select(x => new AddedProduct(x.Id, x.Name, x.Price)).ToList());
            this.AddToUncommitedEvents(@event);
        }
        public void When(OrderCreated @event)
        {
            this.Id = @event.OrderId;
            this.Customer = new Customer(@event.CustomerId, @event.CustomerTitle);

            @event.AddedProducts.ForEach(x =>
            {
                var product = new Product(x.Id, x.Name, x.Price);
                this._orderItems.Add(product);
            });
            this.Sum = OrderItems.Sum(x => x.Price);
        }
        public void When(RemovedProductItem @event)
        {
            var deleteItem = this.OrderItems.FirstOrDefault(x => x.Id == @event.ProductId);
            this._orderItems.Remove(deleteItem);
            this.Sum = OrderItems.Sum(x => x.Price);
        }
        public void When(AddedProductItem @event) {
            this._orderItems.Add(new Product(@event.ProductId, "no-name", @event.Price));
            this.Sum = this.OrderItems.Sum(w => w.Price);
           
        }

    }

}

