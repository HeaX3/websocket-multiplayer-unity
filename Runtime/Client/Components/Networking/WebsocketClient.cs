using System;
using MultiplayerProtocol;
using NativeWebSocket;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Client
{
    public class WebsocketClient : MonoBehaviour, INetworkEndpoint
    {
        public delegate void ClosedEvent(WebSocketCloseCode closeCode);

        public event INetworkEndpoint.CloseEvent closed = delegate { };
        public event INetworkEndpoint.MessageEvent received = delegate { };
        public event ClosedEvent connectionInterrupted = delegate { };

        private WebSocket websocket;

        private bool connectionAttemptDone;
        private Status status;
        private Guid localSessionId;

        public bool isConnected => websocket is { State: WebSocketState.Open };

        private void OnDisable()
        {
            if (websocket != null) websocket.Close();
        }

        public IPromise Connect(string address)
        {
            if (websocket != null)
            {
                return Promise.Rejected(new InvalidOperationException("Already connected"));
            }

            var localSessionId = Guid.NewGuid();
            this.localSessionId = localSessionId;
            status = Status.Connecting;
            connectionAttemptDone = false;

            return new Promise((resolve, reject) =>
            {
                Debug.Log("<b>[Client]</b> Connecting to " + address);
                websocket = new WebSocket(address);
                websocket.OnOpen += () =>
                {
                    if (connectionAttemptDone) return;
                    connectionAttemptDone = true;
                    if (this.localSessionId != localSessionId)
                    {
                        reject(new Exception("Connection attempt interrupted"));
                        return;
                    }

                    if (status != Status.Connecting)
                    {
                        reject(new Exception(
                            "Connection opened, but the client has already entered a different state"));
                        return;
                    }

                    status = Status.Connected;
                    Debug.Log("<b>[Client]</b> Connected to server");
                    resolve();
                };

                websocket.OnError += (e) =>
                {
                    if (connectionAttemptDone)
                    {
                        Debug.LogError(e);
                        return;
                    }

                    connectionAttemptDone = true;
                    status = Status.ConnectionFailed;
                    Debug.Log("<b>[Client]</b> Connection failed");
                    reject(new Exception("Connection attempt failed"));
                };

                websocket.OnClose += code =>
                {
                    Debug.Log("<b>[Client]</b> Connection Closed");
                    if (!connectionAttemptDone)
                    {
                        connectionAttemptDone = true;
                        reject(new Exception("Connection closed prematurely"));
                        return;
                    }

                    if (status != Status.Idle)
                    {
                        status = Status.Idle;
                        websocket = null;
                        connectionInterrupted(code);
                        closed();
                    }
                };

                websocket.OnMessage += bytes =>
                {
                    try
                    {
                        received(new SerializedData(bytes));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                };
                websocket.Connect();
                if (websocket.State != WebSocketState.Connecting && !connectionAttemptDone)
                {
                    connectionAttemptDone = true;
                    reject(new Exception("Connection was terminated prematurely (Encountered status " +
                                         websocket.State + " while connecting)"));
                }
            });
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
#endif
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void Send(SerializedData message)
        {
            if (websocket == null || status != Status.Connected)
            {
                Debug.LogError("<b>[Client]</b> Not connected");
                return;
            }

            websocket.Send(message.ToArray());
            message.Dispose();
        }

        public void Disconnect()
        {
            if (status != Status.Idle && status != Status.ConnectionFailed)
            {
                if (websocket.State != WebSocketState.Closed) websocket.Close();
            }

            status = Status.Idle;
            websocket = null;
            localSessionId = default;
        }

        public static WebsocketClient CreateInstance()
        {
            var gameObject = new GameObject("Websocket Client");
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            return gameObject.AddComponent<WebsocketClient>();
        }

        private enum Status
        {
            Idle,
            Connecting,
            Connected,
            ConnectionFailed
        }
    }
}