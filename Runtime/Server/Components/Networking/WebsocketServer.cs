using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MultiplayerProtocol;
using UnityEngine;
using WebSocketSharp;
using Debug = UnityEngine.Debug;

namespace WebsocketMultiplayer.Server
{
    public class WebsocketServer : MonoBehaviour
    {
        private WebSocketSharp.Server.WebSocketServer wssv;
        private ConcurrentDictionary<string, WebSocket> webSocketConnections { get; } = new();
        private readonly ConcurrentQueue<ReceivedMessageContainer> receivedMessages = new();
        private readonly ConcurrentQueue<Guid> disconnectedUserIds = new();

        private readonly List<ReceivedMessageContainer> receivedMessagesTick = new();
        private readonly List<Guid> disconnectedUserIdsTick = new();

        private IMultiplayerServer server { get; set; }

        private int messagesHandled = 0;
        private float _messagesHandledTime = 0;

        public bool debug { get; set; }
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// IMPORTANT: The generic parameter type of this method must be the same as the generic type of the connection module
        /// </summary>
        public void Initialize<T>(IMultiplayerServer server, Func<IServerNetworkEndpoint, T> connectionCreator)
            where T : NetworkConnection
        {
            this.server = server;

            var ip = IPAddressResolver.Resolve(server.configuration.ipAddress);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Debug.Log("[<b>Server</b>] Starting server...");
                wssv = new WebSocketSharp.Server.WebSocketServer(ip, server.configuration.port);

                // Create a new connection behaviour whenever a client connects
                wssv.AddWebSocketService("/", () =>
                    new ConnectionBehaviour<T>(server, connectionCreator).RegisterHandlers(
                        OnWebSocketConnected,
                        OnWebSocketClosed,
                        OnMessageReceived
                    ));

                wssv.Start();
                Debug.Log("[<b>Server</b>] Multiplayer Server started");
            });

            if (Equals(ip, System.Net.IPAddress.Any))
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 8085);
                    tcpListener.Start();
                    Console.WriteLine($"TcpListener has started on {tcpListener.LocalEndpoint}");
                });
            }
        }

        public void Stop()
        {
            if (wssv != null) wssv.Stop(CloseStatusCode.Away, "Server shutting down");
        }

        private void OnMessageReceived(ConnectionBehaviour connection, MessageEventArgs message)
        {
            var serializedMessage = new SerializedData(message.RawData);
            var messageId = serializedMessage.ReadUShort(moveReadPos: false);
            if (connection.connection.protocol.IsThreadSafeMessage(messageId))
            {
                connection.Receive(serializedMessage);
                return;
            }

            ReceivedMessageContainer resMes;
            resMes.Behaviour = connection;
            resMes.Message = serializedMessage;

            receivedMessages.Enqueue(resMes);
        }

        private void OnWebSocketConnected(ConnectionBehaviour behaviour)
        {
            lock (webSocketConnections)
            {
                webSocketConnections.TryAdd(behaviour.ID, behaviour.Context.WebSocket);
            }

            server.store.connection.AddConnection(behaviour.ID, behaviour);
        }

        private void OnWebSocketClosed(ConnectionBehaviour behaviour, CloseEventArgs closeEventArgs)
        {
            lock (webSocketConnections)
            {
                if (webSocketConnections.TryRemove(behaviour.ID, out var connection))
                {
                    if (connection.IsAlive) connection.Close(CloseStatusCode.Away, "Client web socket stopping");
                    Debug.Log(
                        $"Web Socket Closed {behaviour.ID} disconnected - CloseEventArgs {closeEventArgs.Code} {closeEventArgs.Reason}");
                }
                else
                {
                    Console.WriteLine("Failed removing " + behaviour.ID);
                }
            }

            server.store.connection.RemoveConnection(behaviour.ID);
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (behaviour.connection is IClientConnection clientConnection && clientConnection.userId != default)
            {
                disconnectedUserIds.Enqueue(clientConnection.userId);
            }
        }

        private void OnApplicationQuit()
        {
            wssv.Stop();
        }

        // Update is called once per frame
        void Update()
        {
            // 1. Report disconnected users
            while (disconnectedUserIds.TryDequeue(out var userId))
            {
                disconnectedUserIdsTick.Add(userId);
            }

            foreach (var userId in disconnectedUserIdsTick)
            {
                server.store.connection.ReportDisconnected(userId);
            }

            _messagesHandledTime += Time.deltaTime;
            receivedMessagesTick.Clear();

            // 2. Handle new messages
            while (receivedMessages.TryDequeue(out var action))
            {
                receivedMessagesTick.Add(action);
                messagesHandled++;
            }

            if (debug)
            {
                foreach (var action in receivedMessagesTick.Where(action => action.Behaviour.isAlive))
                {
                    var messageId = action.Message.ReadUShort(false);
                    stopwatch.Reset();
                    stopwatch.Start();
                    action.Behaviour.Receive(action.Message);
                    var time = stopwatch.ElapsedMilliseconds;
                    stopwatch.Stop();
                    if (action.Behaviour.connection.protocol.TryGetMessageType(messageId, out var type))
                    {
                        Debug.Log(type + ": " + time + "ms");
                    }
                }
            }
            else
            {
                foreach (var action in receivedMessagesTick.Where(action => action.Behaviour.isAlive))
                {
                    action.Behaviour.Receive(action.Message);
                }
            }

            receivedMessagesTick.Clear();
            if (_messagesHandledTime >= 5)
            {
                if (debug) Debug.Log("Messages handled in the last 5s: " + messagesHandled);
                messagesHandled = 0;
                _messagesHandledTime = 0;
            }
        }

        public static WebsocketServer CreateInstance()
        {
            var gameObject = new GameObject("Websocket Server");
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            return gameObject.AddComponent<WebsocketServer>();
        }
    }

    public struct ReceivedMessageContainer
    {
        public ConnectionBehaviour Behaviour;
        public SerializedData Message;
    }
}