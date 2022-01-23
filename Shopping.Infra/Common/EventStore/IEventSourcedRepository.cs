namespace Shopping.Infra.Common.EventStore
{
    public interface IEventSourcedRepository<T> where T : IAggregateRoot
    {
        Task<T> GetById(AggregateId id);
        Task<T> GetById(AggregateId id,int version);
  
        Task<IEnumerable<IEvent>> Save(T aggregate, ICommand command);
    }

}
