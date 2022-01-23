using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Domain.OrderAggregate.Entities
{
    public class Customer
    {
        public Customer(Guid id, string title)
        {
            Id = id;
            Title = title;
        }

        public Guid Id { get; set; }    
        public string Title { get; set; }
    }
}
