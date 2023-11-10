using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        internal void ReportDisconnected(Guid userId)
        {
            disconnected(userId);
        }
    }

    public class ConnectionModule<T> : ConnectionModule where T : NetworkConnection
    {
        private readonly ConcurrentDictionary<string, ConnectionBehaviour<T>> connections = new();
        private readonly ConcurrentDictionary<string, Guid> userIdsByConnectionId = new();
        private readonly ConcurrentDictionary<Guid, ConnectionBehaviour<T>> connectionsByUserId = new();

        internal override void AddConnection(string id, ConnectionBehaviour behaviour)
        {
            if (behaviour is not ConnectionBehaviour<T> _behaviour)
            {
                throw new InvalidOperationException("Behaviour has the wrong type! Encountered: " +
                                                    behaviour.GetType().Name + ", Required: " + typeof(T).Name);
            }

            AddConnection(id, _behaviour);
        }

        private void AddConnection(string id, ConnectionBehaviour<T> behaviour)
        {
            lock (connections)
            {
                connections[id] = behaviour;
            }
        }

        internal override void RemoveConnection(string id)
        {
            lock (connections)
            {
                connections.TryRemove(id, out var c);
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
            lock (connections)
            {
                if (!connections.TryGetValue(connectionId, out behaviour))
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
            lock (connections)
            {
                if (!connections.TryGetValue(id, out var behaviour))
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