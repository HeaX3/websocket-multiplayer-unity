using System;
using System.Collections.Concurrent;
using System.Threading;
using MultiplayerProtocol;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketMultiplayer.Server
{
    public abstract class ConnectionBehaviour : WebSocketBehavior, IServerNetworkEndpoint
    {
        public event INetworkEndpoint.CloseEvent closed = delegate { };
        public event INetworkEndpoint.MessageEvent received = delegate { };

        public delegate void ConnectionOpenEvent(ConnectionBehaviour behaviour);

        public delegate void ConnectionCloseEvent(ConnectionBehaviour behaviour, CloseEventArgs e);

        public delegate void MessageReceivedEvent(ConnectionBehaviour behaviour, MessageEventArgs e);

        public event ConnectionOpenEvent connectionOpened = delegate { };
        public event ConnectionCloseEvent connectionClosed = delegate { };
        public event MessageReceivedEvent messageReceived = delegate { };

        private readonly ConcurrentQueue<PendingMessage> sendQueue = new();

        private readonly IMultiplayerServer _server;
        private bool _sending;

        public abstract NetworkConnection connection { get; }

        public bool isAlive => Context.WebSocket.IsAlive;
        public bool isOpen => isAlive;

        IMultiplayerServer IServerNetworkEndpoint.server => _server;
        string IServerNetworkEndpoint.id => ID;

        protected ConnectionBehaviour(IMultiplayerServer server)
        {
            _server = server;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            StartSending();
            connectionOpened(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            StopSending();
            connectionClosed(this, e);
            closed();
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Debug.LogError("ERROR " + e.Exception + " " + e.Message);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            messageReceived(this, e);
        }

        internal void Receive(SerializedData message)
        {
            received(message);
        }

        private void StartSending()
        {
            _sending = true;
            sendQueue.Clear();
            var sendThread = new Thread(() =>
            {
                while (_sending)
                {
                    while (sendQueue.TryDequeue(out var message))
                    {
                        if (message.expiration != default && DateTime.UtcNow > message.expiration) continue;
                        Send(message.data);
                    }

                    Thread.Sleep(10);
                }
            });
            sendThread.Start();
        }

        private void StopSending()
        {
            _sending = false;
        }

        void INetworkEndpoint.Send(SerializedData message, DateTime expiration)
        {
            sendQueue.Enqueue(new PendingMessage(message.ToArray(), expiration));
        }

        public ConnectionBehaviour RegisterHandlers(Action<ConnectionBehaviour> opened,
            Action<ConnectionBehaviour, CloseEventArgs> closed,
            Action<ConnectionBehaviour, MessageEventArgs> messageReceived)
        {
            this.connectionOpened += opened.Invoke;
            this.connectionClosed += closed.Invoke;
            this.messageReceived += messageReceived.Invoke;
            return this;
        }
    }

    public class ConnectionBehaviour<T> : ConnectionBehaviour where T : NetworkConnection
    {
        public override NetworkConnection connection => _connection;
        internal T _connection { get; }

        public ConnectionBehaviour(IMultiplayerServer server, Func<IServerNetworkEndpoint, T> connection) : base(server)
        {
            _connection = connection(this);
        }
    }

    readonly struct PendingMessage
    {
        public readonly byte[] data;
        public readonly DateTime expiration;

        public PendingMessage(byte[] data, DateTime expiration)
        {
            this.data = data;
            this.expiration = expiration;
        }
    }
}