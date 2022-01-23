namespace Shopping.Infra.Common
{
    public interface IAggregateId
    {
        string Value { get; }
    }
    public class GuidId : AggregateId
    {
        public GuidId(Guid value) : base(value)
        {
        }

    }
    public abstract class AggregateId : IAggregateId
    {
        protected AggregateId()
        {
        }
        protected AggregateId(object id)
        {
            Value = id.ToString();
        }
        public string Value { get; protected set; }
        public static implicit operator AggregateId(Guid guidId)
        {
            return new GuidId(guidId);
        }

        public static implicit operator Guid(AggregateId id)
        {
            Guid ret;
            var isSuccessful = Guid.TryParse(id.Value.ToString(), out ret);
            if (isSuccessful)
            {
                return ret;
            }
            else
            {
                throw new InvalidCastException("the value of the Id property could not be converted to a guid");
            }
        }

    }
}
