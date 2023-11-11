using System;
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

        private readonly IMultiplayerServer _server;

        public abstract NetworkConnection connection { get; }

        public bool isAlive => Context.WebSocket.IsAlive;

        IMultiplayerServer IServerNetworkEndpoint.server => _server;
        string IServerNetworkEndpoint.id => ID;

        protected ConnectionBehaviour(IMultiplayerServer server)
        {
            _server = server;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            connectionOpened(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
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

        void INetworkEndpoint.Send(SerializedData message)
        {
            var data = message.ToArray();
            Send(data);
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
}