using Shopping.Infra.Common.ReadStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Persistance.ReadStore
{
    public class OrderDetailView : BaseEntity<Guid>
    {
        public OrderDetailView()
        {
        }
        public double Sum { get; set; }
        public virtual List<OrderItemView> OrderItemViews { get; set; }=new List<OrderItemView>();
    }
    public class OrderItemView : BaseViewEntity<Guid>
    {
        public OrderItemView()
        {

        }
        public Guid OrderId { get; set; }
        public virtual OrderDetailView OrderDetailView { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
    }
}
