using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Infra.Common.ReadStore
{
    public interface IView
    {
        long Version { get; set; }
    }

    public interface IView<out TId> : IView
    {
        TId Id { get; }
    }

    public abstract class SingletonView : IView<string>
    {
        public string Id
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public abstract long Version { get; set; }
    }
}
