using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DataStores;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Server
{
    public abstract class ConnectionModule : IDataModule
    {
        public delegate void DisconnectedEvent(Guid userId);

        public event DisconnectedEvent disconnected = delegate { };

        public void Initialize()
        {
        }

        internal abstract void AddConnection(string id, ConnectionBehaviour behaviour);
        internal abstract void RemoveConnection(string id);
        internal abstract void SetUserId(string connectionId, Guid userId);
        public abstract IReadOnlyList<NetworkConnection> GetConnections();

        internal void ReportDisconnected(Guid userId)
        {
            disconnected(userId);
        }
    }

    /// <summary>
    /// IMPORTANT: The generic parameter type of this method must be the same as the generic type of the websocket server initializer
    /// </summary>
    public class ConnectionModule<T> : ConnectionModule where T : NetworkConnection
    {
        private readonly ConcurrentDictionary<string, ConnectionBehaviour<T>> _connections = new();
        private readonly ConcurrentDictionary<string, Guid> userIdsByConnectionId = new();
        private readonly ConcurrentDictionary<Guid, ConnectionBehaviour<T>> connectionsByUserId = new();

        public IReadOnlyDictionary<Guid, T> connections
        {
            get
            {
                Dictionary<Guid, T> result;
                lock (connectionsByUserId)
                {
                    result = connectionsByUserId.ToDictionary(e => e.Key, e => e.Value._connection);
                }

                return result;
            }
        }

        public override IReadOnlyList<NetworkConnection> GetConnections()
        {
            List<NetworkConnection> result;
            lock (_connections)
            {
                result = _connections.Values.Select(c => c.connection).ToList();
            }

            return result;
        }

        internal override void AddConnection(string id, ConnectionBehaviour behaviour)
        {
            if (behaviour is not ConnectionBehaviour<T> _behaviour)
            {
                throw new InvalidOperationException(
                    "Behaviour has the wrong connection type! Encountered: " +
                    behaviour.GetType().GenericTypeArguments.FirstOrDefault()?.Name +
                    ", Required: " + typeof(T).Name + "\n" +
                    "Make sure you create this module with the exact same connection type as you return in your " +
                    "connection creator in the websocket server!");
            }

            AddConnection(id, _behaviour);
        }

        private void AddConnection(string id, ConnectionBehaviour<T> behaviour)
        {
            lock (_connections)
            {
                _connections[id] = behaviour;
            }
        }

        internal override void RemoveConnection(string id)
        {
            lock (_connections)
            {
                _connections.TryRemove(id, out var c);
            }

            lock (userIdsByConnectionId)
            {
                if (userIdsByConnectionId.TryRemove(id, out var previousUserId))
                {
                    lock (connectionsByUserId)
                    {
                        connectionsByUserId.Remove(previousUserId, out _);
                    }
                }
            }
        }

        internal override void SetUserId(string connectionId, Guid userId)
        {
            ConnectionBehaviour<T> behaviour;
            lock (_connections)
            {
                if (!_connections.TryGetValue(connectionId, out behaviour))
                {
                    throw new InvalidOperationException("Connection " + connectionId + " not found");
                }
            }

            SetUserId(behaviour, userId);
        }

        private void SetUserId(ConnectionBehaviour<T> behaviour, Guid userId)
        {
            lock (userIdsByConnectionId)
            {
                lock (connectionsByUserId)
                {
                    if (userIdsByConnectionId.TryRemove(behaviour.ID, out var previousUserId))
                    {
                        connectionsByUserId.Remove(previousUserId, out _);
                    }

                    userIdsByConnectionId[behaviour.ID] = userId;
                    connectionsByUserId[userId] = behaviour;
                }
            }

            if (behaviour.connection is IClientConnection clientConnection) clientConnection.userId = userId;
        }

        internal bool TryGetConnection(string id, out T connection)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(id, out var behaviour))
                {
                    connection = default;
                    return false;
                }

                connection = behaviour._connection;
                return true;
            }
        }

        public bool TryGetConnection(Guid userId, out T connection)
        {
            lock (connectionsByUserId)
            {
                if (!connectionsByUserId.TryGetValue(userId, out var behaviour))
                {
                    connection = default;
                    return false;
                }

                connection = behaviour._connection;
                return true;
            }
        }
    }
}