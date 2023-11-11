using System;
using DataStores;
using MultiplayerProtocol;
using NativeWebSocket;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Client.Modules
{
    public class ConnectionModule : IDataModule
    {
        public delegate void ConnectionEvent();

        public delegate void WebsocketConnectionEvent(WebSocketCloseCode reason);

        public event ConnectionEvent connected = delegate { };
        public event ConnectionEvent disconnected = delegate { };
        public event WebsocketConnectionEvent websocketDisconnected = delegate { };

        private IMultiplayerClient client { get; }

        public ConnectionModule(IMultiplayerClient client)
        {
            this.client = client;
        }

        public void Initialize()
        {
            client.client.connectionInterrupted += reason =>
            {
                disconnected();
                websocketDisconnected(reason);
            };
        }

        /// <summary>
        /// <list type="number">
        /// <item>Try to read credentials from the local storage</item>
        /// <item>Perform OAuth flow if no credentials are present</item>
        /// <item>Connect to the server</item>
        /// <item>Perform authentication handshake</item>
        /// </list>
        /// </summary>
        public IPromise AuthenticateAndConnect()
        {
            return new Promise((resolve, reject) =>
            {
                PerformLogin().Then(() =>
                {
                    client.SetLoadingStatus("Connecting...");
                    var secret = client.store.auth.secret;
                    Connect().Then(joinResult =>
                    {
                        client.store.auth.userId = joinResult.userId.value;
                        client.store.auth.secret = secret;

                        client.SetLoadingStatus(null);
                        connected();
                        resolve();
                    }).Catch(e =>
                    {
                        client.SetLoadingStatus(null);
                        if (e is ForbiddenException) client.store.auth.Reset();
                        reject(e);
                    });
                }).Catch(reject);
            });
        }

        /// <summary>
        /// <list type="number">
        /// <item>Try to read credentials from the local storage</item>
        /// <item>Perform OAuth flow if no credentials are present</item>
        /// <item>Connect to the server</item>
        /// <item>Perform authentication handshake</item>
        /// <item>Return response message of type <see cref="T"/></item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">Response message type</typeparam>
        /// <returns>Initial application state provided by the server after successfully authenticating the user</returns>
        public IPromise<T> AuthenticateAndConnect<T>() where T : ISerializableValue, new()
        {
            return new Promise<T>((resolve, reject) =>
            {
                PerformLogin().Then(() =>
                {
                    client.SetLoadingStatus("Connecting...");
                    var secret = client.store.auth.secret;
                    Connect().Then(joinResult =>
                    {
                        client.store.auth.userId = joinResult.userId.value;
                        client.store.auth.secret = secret;

                        var extra = new T();
                        extra.DeserializeFrom(
                            new SerializedData(joinResult.extra.value ?? Array.Empty<byte>())
                        );
                        client.SetLoadingStatus(null);
                        connected();
                        resolve(extra);
                    }).Catch(e =>
                    {
                        client.SetLoadingStatus(null);
                        if (e is ForbiddenException) client.store.auth.Reset();
                        reject(e);
                    });
                }).Catch(reject);
            });
        }

        public void Disconnect()
        {
            client.client.Disconnect();
        }

        private IPromise PerformLogin()
        {
            return new Promise((resolve, reject) =>
            {
                client.store.auth.FetchCredentials();
                if (client.store.auth.userId != null)
                {
                    resolve();
                    return;
                }

                client.SetLoadingStatus("Authenticate...");
                client.login.PromptLogin().Then(info =>
                {
                    client.store.auth.userId = info.Key;
                    client.store.auth.secret = info.Value;
                    client.store.auth.SaveCredentials();
                    resolve();
                }).Catch(reject);
            });
        }

        private IPromise<AuthResultMessage> Connect()
        {
            var userId = client.store.auth.userId;
            var secret = client.store.auth.secret;
            client.store.auth.userId = default;
            client.store.auth.secret = default;
            return Connect(userId, secret);
        }

        private IPromise<AuthResultMessage> Connect(Guid userId, string token)
        {
            return new Promise<AuthResultMessage>((resolve, reject) =>
            {
                client.client.Connect(client.serverAddress).Then(() =>
                {
                    Debug.Log("[<b>Client</b>] Connection successful, performing protocol handshake");
                    client.connection.PerformProtocolHandshake().Then(() =>
                    {
                        Debug.Log("[<b>Client</b>] Protocol established, performing authentication");
                        client.connection.auth.Authenticate(userId, token).Then(resolve).Catch(reject);
                    }).Catch(reject);
                }).Catch(reject);
            });
        }
    }
}