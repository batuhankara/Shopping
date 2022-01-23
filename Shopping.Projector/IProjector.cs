using EventStore.ClientAPI;

public interface IProjector
{
    void Process(EventStoreCatchUpSubscription sub, ResolvedEvent resolvedEvent);
}
