using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;

namespace Shopping.Infra.Common.EventStore
{
    public interface IEventStoreConnectionManager
    {

        IEventStoreConnection GetConnection();

    }
    public class EventStoreConnectionManager : IEventStoreConnectionManager, IDisposable
    {
        protected readonly IConfiguration _configuration;

        private IEventStoreConnection _eventStoreConnection;
        private readonly object _lock = new object();

        public EventStoreConnectionManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {

        }

        public IEventStoreConnection GetConnection()
        {
            if (_eventStoreConnection != null) return _eventStoreConnection;

            lock (_lock)
            {
                SetupNewConnection().GetAwaiter().GetResult();
            }

            _eventStoreConnection.Closed -= EventStoreConnection_Closed;
            _eventStoreConnection.Closed += EventStoreConnection_Closed;
            _eventStoreConnection.Connected -= EventStoreConnection_Connection;
            _eventStoreConnection.Connected += EventStoreConnection_Connection;
            _eventStoreConnection.Reconnecting -= EventStoreConnection_Reconnecting;
            _eventStoreConnection.Reconnecting += EventStoreConnection_Reconnecting;
            _eventStoreConnection.ErrorOccurred -= EventStoreConnection_Error;
            _eventStoreConnection.ErrorOccurred += EventStoreConnection_Error;
            _eventStoreConnection.AuthenticationFailed -= EventStoreConnection_Authentication_Failed;
            _eventStoreConnection.AuthenticationFailed += EventStoreConnection_Authentication_Failed;
            return _eventStoreConnection;
        }
        private async Task SetupNewConnection()
        {
            var connectionUri = _configuration["ConnectionStrings:EventStore"];
            var settings = ConnectionSettings
      .Create()
      .KeepReconnecting()
      .EnableVerboseLogging().
      UseConsoleLogger()
  .DisableServerCertificateValidation().DisableTls()
      .Build();

            _eventStoreConnection = EventStoreConnection.Create(
                settings, new Uri(connectionUri)
            );

            await _eventStoreConnection.ConnectAsync();

        }
        #region EventStore connection events
        private void EventStoreConnection_Connection(object sender, ClientConnectionEventArgs e)
        {
            //_logger.Information("EventStore connection event");
        }

        private void EventStoreConnection_Closed(object sender, ClientClosedEventArgs e)
        {
            _eventStoreConnection = null;
            //_logger.Information($"EventStore connection closed: {e.Reason}");
        }

        private void EventStoreConnection_Reconnecting(object sender, ClientReconnectingEventArgs e)
        {
        }

        private void EventStoreConnection_Error(object sender, ClientErrorEventArgs e)
        {
        }

        private void EventStoreConnection_Authentication_Failed(object sender, ClientAuthenticationFailedEventArgs e)
        {
        }
        #endregion
    }
}
