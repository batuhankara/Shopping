using Newtonsoft.Json;
using Shopping.Infra.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonConstructorAttribute = Newtonsoft.Json.JsonConstructorAttribute;

namespace Shopping.Common.Messaging.Order.Events
{
    public class AddedProduct
    {
        public AddedProduct(Guid id, string name, double price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
    public class OrderCreated : BaseEvent
    {
        [JsonProperty]
        public Guid OrderId { get;  }
        public double Sum { get; }
        public string CustomerTitle { get; }
        public Guid CustomerId { get; }
        public List<AddedProduct> AddedProducts { get; }


        public OrderCreated(Guid orderId, double sum, string customerTitle, Guid customerId, List<AddedProduct> addedProducts)
        {
            OrderId = orderId;
            Sum = sum;
            CustomerTitle = customerTitle;
            CustomerId = customerId;
            AddedProducts = addedProducts;
        }
    }
}
