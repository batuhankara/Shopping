using Shopping.Infra.Common;

namespace Shopping.Common.Messaging.Order.Events
{
    public class AddedProductItem : BaseEvent
    {
        public AddedProductItem(Guid orderId, Guid productId, double price)
        {
            OrderId = orderId;
            ProductId = productId;
            Price = price;
        }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
    }
}
