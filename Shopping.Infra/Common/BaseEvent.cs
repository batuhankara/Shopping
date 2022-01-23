namespace Shopping.Infra.Common
{
    public class BaseEvent : IEvent
    {
        public BaseEvent()
        {
            this.EventId = Guid.NewGuid();
            this.OccuredOnUTC = DateTime.Now;
        }
        public Guid EventId { get; private set; }

        public DateTime OccuredOnUTC { get; private set; }
    }
}
