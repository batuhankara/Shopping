using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Common.Messaging.Order.Dtos
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }   
        public List<OrderProductDetailDto> OrderProductDetailDtos { get; set; }
        public double Sum { get; set; }
    }
    public class OrderProductDetailDto
    {
        public Guid ProductId { get; set; }
        public double Price { get; set; }   
    }
}
