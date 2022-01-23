namespace Shopping.Infra.Common
{
    public interface IEvent
    {

        Guid EventId { get; }
        DateTime OccuredOnUTC { get; }
    }
}
