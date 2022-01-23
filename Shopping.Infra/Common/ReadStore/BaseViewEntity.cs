using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shopping.Infra.Common.ReadStore
{
 
    public abstract class BaseViewEntity<T> : BaseEntity<T>, IView<T>
    {
        public long Version
        {
            get; set;
        }
    }
    public abstract class BaseEntity<TId>
    {
        public BaseEntity()
        {
            CreatedAtUTC = DateTime.UtcNow;
        }


        public TId Id { get; set; }


        public DateTime CreatedAtUTC { get; set; }

        public DateTime? UpdatedAtUTC { get; set; }

     
    }
}


  
