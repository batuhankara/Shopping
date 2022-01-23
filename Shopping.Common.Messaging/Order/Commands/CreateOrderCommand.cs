using Shopping.Infra.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Common.Messaging.Order.Commands
{
    public class RemoveOrderItemCommand : BaseCommand
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
    }
    public class CreateOrderCommand : BaseCommand
    {
        public Guid OrderId { get; set; }
        public AddCustomerCommand AddCustomerCommand { get; set; }
        public List<AddProductToOrderCommand> AddProductToOrderCommands { get; set; }

        public CreateOrderCommand()
        {
        }

        public CreateOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
    public class AddCustomerCommand : BaseCommand
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
    public class AddProductToOrderCommand : BaseCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
