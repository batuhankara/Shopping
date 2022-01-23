using EventStore.ClientAPI;
using Newtonsoft.Json;
using Shopping.Common.Extensions;
using System.Text;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Shopping.Infra.Common.EventStore
{
    public class EventStoreRepository<T> : IEventSourcedRepository<T> where T : EventSourcedAggregate
    {
        private IEventStoreConnection eventStoreConnection;

        public EventStoreRepository(IEventStoreConnectionManager eventStoreConnectionManager)
        {
            this.eventStoreConnection = eventStoreConnectionManager.GetConnection();

        }

        public async Task<T> GetById(AggregateId id, int version)
        {
            var aggregate = ConstructAggregate();
            var streamName = aggregate.GetStreamName(id);
            if (version <= 0) throw new InvalidOperationException("Cannot get version <= 0");

            long sliceStart = 0;
            StreamEventsSlice currentSlice;

            int ReadPageSize = 500;

            do
            {
                int sliceCount = sliceStart + ReadPageSize <= version
                                    ? ReadPageSize
                                    : (int)(version - sliceStart + 1);


                currentSlice = await eventStoreConnection.ReadStreamEventsForwardAsync(streamName, sliceStart, sliceCount, true);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
#pragma warning disable CS8603 // Possible null reference return.
                    return null;
#pragma warning restore CS8603 // Possible null reference return.

                sliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    if (evnt.Event == null || evnt.Event.Data == null || evnt.Event.Metadata == null)
                        continue;

                    var eventAndMetadata = DeserializeEvent(evnt.Event.Metadata, evnt.Event.Data);

                    if (eventAndMetadata == null)
                        continue;

                    aggregate.ApplyEvent(eventAndMetadata.Item1, eventAndMetadata.Item2);
                }
            } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);



            // _workContext.TelemetryClient.StopOperation(telemetryOp, false, false);

            return aggregate;
        }
        public async Task<T> GetById(AggregateId id)
        {
            return await GetById(id, int.MaxValue);
        }


        public async Task<IEnumerable<IEvent>> Save(T aggregate, ICommand command)
        {
            if (command.CommandIdentityToken == Guid.Empty)
            {
                throw new ArgumentException("A non-empty guid must be provided as a command identity token");
            }

            var streamName = aggregate.GetStreamName(aggregate.GetAggregateId());

            var newEvents = aggregate.GetUncommittedEvents().ToList();
            var originalVersion = aggregate.Version - newEvents.Count;
            var expectedVersion = originalVersion == -1 ? ExpectedVersion.NoStream : originalVersion;
            var commitHeaders = new Dictionary<string, object>();
            commitHeaders.Add("CommandIdentityToken", command.CommandIdentityToken.ToString());
            commitHeaders.Add("CommandType", command.GetType().FullName);


            var eventsToSave = newEvents.Select((evt, idx) =>
            {
                //generate an eventid for each event; use deterministic guids derived from command identity token and index
                var eventId = DeterministicGuidGenerator.Create(command.CommandIdentityToken, $"event-{idx}");
                Console.WriteLine(evt);
                return ToEventData(eventId, evt, commitHeaders);
            }).ToList();

            var WritePageSize = 500;


            try
            {
                if (eventsToSave.Count() < WritePageSize)
                {
                    var res = await eventStoreConnection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
                    //var res = await _eventStoreConnection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
                }
                else
                {
                    var transaction = await eventStoreConnection.StartTransactionAsync(streamName, expectedVersion);

                    var position = 0;
                    while (position < eventsToSave.Count())
                    {
                        var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);
                        await transaction.WriteAsync(pageEvents);
                        position += WritePageSize;
                    }

                    //await transaction.CommitAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {

            }


            aggregate.ClearUncommittedEvents();

            //all "newevents" are commited at this point.
            return newEvents;
        }





        private static Dictionary<string, RuntimeTypeHandle> _cache = new Dictionary<string, RuntimeTypeHandle>();
        private Type GetTypeCached(string clrTypeName)
        {
            if (_cache.ContainsKey(clrTypeName))
            {
                return Type.GetTypeFromHandle(_cache[clrTypeName]);
            }
            else
            {
                var ret = Type.GetType(clrTypeName);

                if (ret == null)
                    return null;

                _cache[clrTypeName] = ret.TypeHandle;

                return ret;
            }
        }

        private Tuple<object, Dictionary<string, object>> DeserializeEvent(byte[] metadata, byte[] data)
        {
            var stringMetadata = Encoding.UTF8.GetString(metadata);
            var headers = JsonConvert.DeserializeObject<Dictionary<string, object>>(stringMetadata);
            string clrTypeName = headers[EventClrTypeHeader] as string;
            var stringData = Encoding.UTF8.GetString(data);
            var eventType = GetTypeCached(clrTypeName);
            if (eventType == null)
                return null;
            var @event = JsonConvert.DeserializeObject(stringData, eventType, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            return new Tuple<object, Dictionary<string, object>>(@event, headers);
        }
        private static string EventClrTypeHeader = "EventClrTypeName";
        private EventData ToEventData(Guid eventId, IEvent evnt, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt));
            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName},
            };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders));
            var typeName = evnt.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }
        private T ConstructAggregate()
        {
            Type t = typeof(T);
            return (T)Activator.CreateInstance(t, true);
        }
    }

}
