using Shopping.Infra.Common;

namespace Shopping.Common.Messaging.Order.Events
{
    public class RemovedProductItem : BaseEvent
    {
        public RemovedProductItem(Guid orderId, Guid productId)
        {
            OrderId = orderId;
            ProductId = productId;
        }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
    }
}
