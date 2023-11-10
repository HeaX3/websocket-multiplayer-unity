using System;
using MultiplayerProtocol;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketMultiplayer.Server
{
    public abstract class ConnectionBehaviour : WebSocketBehavior, IServerNetworkEndpoint
    {
        public event INetworkEndpoint.MessageEvent received = delegate { };

        public delegate void ConnectionOpenEvent(ConnectionBehaviour behaviour);

        public delegate void ConnectionCloseEvent(ConnectionBehaviour behaviour, CloseEventArgs e);

        public delegate void MessageReceivedEvent(ConnectionBehaviour behaviour, MessageEventArgs e);

        public event ConnectionOpenEvent opened = delegate { };
        public event ConnectionCloseEvent closed = delegate { };
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
            opened(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            closed(this, e);
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

        internal void Receive(SerializedMessage message)
        {
            received(message);
        }

        void INetworkEndpoint.Send(SerializedMessage message)
        {
            var data = message.ToArray();
            Send(data);
        }

        public ConnectionBehaviour RegisterHandlers(Action<ConnectionBehaviour> opened,
            Action<ConnectionBehaviour, CloseEventArgs> closed,
            Action<ConnectionBehaviour, MessageEventArgs> messageReceived)
        {
            this.opened += opened.Invoke;
            this.closed += closed.Invoke;
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