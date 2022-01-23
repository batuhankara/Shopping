using Shopping.Infra.Common;

namespace Shopping.Common.Messaging.Order.Commands
{
    public class AddProductCommand : BaseCommand
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
