using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shopping.Infra.Common.EventStore;
using System.Text;
using Newtonsoft.Json;
using Shopping.Common.Messaging.Order.Events;
using Shopping.Persistance.ReadStore.DbContext;
using Shopping.Persistance.ReadStore;
using Microsoft.EntityFrameworkCore;
using ReflectionMagic;

Console.WriteLine("Hello, World!");
EventStoreCatchUpSubscription _sub;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddTransient<IEventStoreConnectionManager, EventStoreConnectionManager>()
        .AddTransient<IProjector, OrderProjector>()
.AddDbContext<OrderDbContext>(

        options => options.UseSqlServer("Server=mssqlDB,1433;Database=SHOPPINGDB;User=sa;Password=Your_password123;")))


    .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

var connection = provider.GetRequiredService<IEventStoreConnectionManager>().GetConnection();
var projector = provider.GetRequiredService<IProjector>();


//    connection.SubscribeToAllFrom(
//    lastCheckpoint: Position.Start,
//    settings: new CatchUpSubscriptionSettings(int.MaxValue, 500, verboseLogging: false, resolveLinkTos: true, string.Empty),
//    eventAppeared: (sub, evt)
//        => projector.Process(sub, evt),
//    liveProcessingStarted: sub
//        => Console.WriteLine("Processing live"),
//    subscriptionDropped: (sub, reason, exception)
//        => Console.WriteLine($"Subscription dropped: {reason}")
//);
try
{
    var sub = new Sub(connection, projector);
    sub.Subscribe();
    while (true)
    {

    }
}
catch (Exception e)
{

    throw;
}
public class Sub
{
    private EventStoreCatchUpSubscription _sub { get; set; }
    private IEventStoreConnection _connection { get; set; }
    private IProjector _projector { get; set; }
    public Sub(IEventStoreConnection connect, IProjector projector)
    {
        _connection = connect;
        _projector = projector;
    }
    public void Subscribe()
    {
        _sub = _connection.SubscribeToAllFrom(
        lastCheckpoint: Position.End,
        settings: new CatchUpSubscriptionSettings(int.MaxValue, 500, verboseLogging: false, resolveLinkTos: true, string.Empty),
        eventAppeared: (sub, evt)
            => _projector.Process(sub, evt),
        liveProcessingStarted: sub
            => Console.WriteLine("Processing live"),
        subscriptionDropped: (sub, reason, exception)
            => SubscriptionDropped(sub, reason, exception));


    }
    void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
    {

        _sub.Stop();
        Subscribe();
    }
}
public class OrderProjector : IProjector
{
    private readonly OrderDbContext _orderDbContext;

    public OrderProjector(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public void Process(EventStoreCatchUpSubscription sub, ResolvedEvent resolvedEvent)
    {
        if (!resolvedEvent.OriginalStreamId.Contains("$"))
        {

            var stringMetadata = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
            var headers = JsonConvert.DeserializeObject<Dictionary<string, object>>(stringMetadata);
            string clrTypeName = headers["EventClrTypeName"] as string;
            var stringData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            var eventType = Type.GetType(clrTypeName);
            if (eventType == null)
                return;
            var @event = JsonConvert.DeserializeObject(stringData, eventType, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            this.AsDynamic().When(@event, resolvedEvent.Event.EventNumber);

        }

    }
    public void When(OrderCreated orderCreated, long version)
    {
        try
        {
            var order = _orderDbContext.OrderDetailView.FirstOrDefault(x => x.Id == orderCreated.OrderId);
            if (order != null) return;
            var entity = new OrderDetailView()
            {
                Id = orderCreated.OrderId,
                OrderItemViews = orderCreated.AddedProducts.Select(x => new OrderItemView()
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderCreated.OrderId,
                    Price = x.Price,
                    ProductId = x.Id

                }).ToList(),
                Sum = orderCreated.Sum

            };
            _orderDbContext.OrderDetailView.Add(entity);
            _orderDbContext.SaveChanges();

        }
        catch (Exception e)
        {

            throw;
        }


    }
    public void When(RemovedProductItem removedProductItem, long version)
    {
        var entity = _orderDbContext.OrderItemView.FirstOrDefault(x => x.OrderId == removedProductItem.OrderId && x.ProductId == removedProductItem.ProductId);
        if (entity != null)
        {
            _orderDbContext.OrderItemView.Remove(entity);
            _orderDbContext.SaveChanges();
        }

    }
    public void When(AddedProductItem addedProductItem, long version)
    {
        var entity = _orderDbContext.OrderDetailView.FirstOrDefault(x => x.Id == addedProductItem.OrderId);
        if (entity != null)
        {
            if (entity.OrderItemViews == null)
            {
                entity.OrderItemViews = new List<OrderItemView>();
            }
            _orderDbContext.OrderItemView.Add(new OrderItemView()
            {
                Id = Guid.NewGuid(),
                ProductId = addedProductItem.ProductId,
                OrderId = addedProductItem.OrderId,
                Price = addedProductItem.Price
            });
            _orderDbContext.SaveChanges();
        }

    }


}