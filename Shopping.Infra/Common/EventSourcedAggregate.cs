
using Newtonsoft.Json;
using ReflectionMagic;

namespace Shopping.Infra.Common
{
    public abstract class EventSourcedAggregate : IAggregateRoot
    {

        private List<IEvent> _uncommitedEvents = new List<IEvent>();
        private int _version = -1;

        private GuidId _id;
        protected virtual AggregateId Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value is GuidId)
                {
                    _id = (GuidId)value;
                }
                else
                {
                    throw new InvalidOperationException("override this property if you wish to use anything other than GuidId as its assignable type.");
                }
            }
        }
        public AggregateId GetAggregateId()
        {
            return Id;
        }

        public IEnumerable<IEvent> Handle<TCommand>(TCommand command, params object[] args) where TCommand : BaseCommand
        {
            if (this.HasPreviouslyExecutedCommand(command))
            {
                IEnumerable<IEvent> previousResult = GetEventsProducedInResponseToCommand(command);
                this.AddToUncommitedEvents(previousResult);

            }
            else
            {
                switch (args.Length)
                {
                    case 0:
                        this.AsDynamic().Handle(command);
                        break;
                    case 1:
                        this.Handle(command, args[0]);
                        break;
                    case 2:
                        this.Handle(command, args[0], args[1]);
                        break;
                    case 3:
                        this.Handle(command, args[0], args[1], args[2]);
                        break;
                    case 4:
                        this.Handle(command, args[0], args[1], args[2], args[3]);
                        break;
                    default:
                        throw new NotSupportedException("more than 4 parameters are not supported... yet...");
                }

            }

            return this.GetUncommittedEvents();
        }

        public int Version { get { return _version; } protected set { _version = value; } }

        internal void ApplyEvent(object @event, Dictionary<string, object> metadata)
        {
            string commandIdentityTokenAsString = metadata["CommandIdentityToken"] as string;
            string typeOfCommand = metadata.ContainsKey("CommandType") ? metadata["CommandType"] as string : string.Empty;

            var commandIdentityToken = string.IsNullOrEmpty(commandIdentityTokenAsString) ? Guid.NewGuid() : Guid.Parse(commandIdentityTokenAsString);

            AddEventToCommandAndEventsHistory(@event as IEvent, commandIdentityToken, typeOfCommand);
            this.AsDynamic().When(@event);
            Version++;
        }
        internal void ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }

        internal IEnumerable<IEvent> GetUncommittedEvents()
        {
            return _uncommitedEvents;
        }

        protected void AddToUncommitedEvents(IEnumerable<IEvent> events)
        {
            _uncommitedEvents.AddRange(events);
            Version += events.Count();
        }

        protected void AddToUncommitedEvents(IEvent @event)
        {
            AddToUncommitedEvents(new IEvent[] { @event });
        }

    
        protected void When(object @event)
        {
            //aggregate tekrar insa edilirken, spesifik event handler bulunmazsa bu method cagiriliyor - silmeyin
        }

        private Dictionary<Tuple<string, Guid>, List<IEvent>> _commandAndEventsHistory = new Dictionary<Tuple<string, Guid>, List<IEvent>>();

        private Tuple<string, Guid> GetCommandIdentityKey(ICommand command)
        {
            return new Tuple<string, Guid>(command.GetType().FullName, command.CommandIdentityToken);
        }


        private void AddEventToCommandAndEventsHistory(IEvent @event, Guid commandIdentityToken, string typeOfCommand)
        {
            var key = new Tuple<string, Guid>(typeOfCommand, commandIdentityToken);
            if (_commandAndEventsHistory.ContainsKey(key))
            {
                _commandAndEventsHistory[key].Add(@event);
            }
            else
            {
                _commandAndEventsHistory.Add(key, new List<IEvent>() { @event });
            }
        }

        private void AddEventToCommandAndEventsHistory(IEvent @event, ICommand command)
        {
            var key = GetCommandIdentityKey(command);
            if (_commandAndEventsHistory.ContainsKey(key))
            {
                _commandAndEventsHistory[key].Add(@event);
            }
            else
            {
                _commandAndEventsHistory.Add(key, new List<IEvent>() { @event });
            }
        }

        public bool HasPreviouslyExecutedCommand(ICommand command)
        {
            var key = GetCommandIdentityKey(command);
            return _commandAndEventsHistory.ContainsKey(key);
        }

        protected IEnumerable<IEvent> EventStream => _commandAndEventsHistory.SelectMany(x => x.Value);

        public IEnumerable<IEvent> GetEventsProducedInResponseToCommand(ICommand command)
        {
            var key = GetCommandIdentityKey(command);
            return _commandAndEventsHistory.ContainsKey(key) ? _commandAndEventsHistory[key] : null;
        }

        public virtual string GetStreamName(AggregateId id)
        {
            return GetFirstNonAbstractTypeInInheritenceHierarchy(this.GetType()).Name + "-" + id.Value.ToString();
        }

        private static Type GetFirstNonAbstractTypeInInheritenceHierarchy(Type aDerivedType)
        {
            var baseType = aDerivedType.BaseType;
            if (baseType != null && !baseType.IsAbstract)
            {
                return GetFirstNonAbstractTypeInInheritenceHierarchy(baseType);
            }
            else
            {
                return aDerivedType;
            }
        }
    }
}



