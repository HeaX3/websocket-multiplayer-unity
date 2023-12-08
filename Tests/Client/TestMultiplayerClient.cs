using System;
using System.Collections.Generic;
using FileStore;
using RSG;
using UnityEngine;
using WebsocketMultiplayer.Client;

namespace WebsocketMultiplayer.Tests.Client
{
    public class TestMultiplayerClient : IMultiplayerClient, ILoginClient
    {
        public string serverAddress { get; }
        public WebsocketClient client { get; private set; }
        public IMultiplayerStore store { get; private set; }
        public IFileStorage files { get; } = new MemoryStorage();
        public TestClientConnection connection { get; private set; }
        IMultiplayerConnection IMultiplayerClient.connection => connection;
        public ILoginClient login => this;
        public ILoginApi api { get; }
        public string platform => "test";
        public ILoginHandler authentication { get; } = new DummyLoginHandler();

        public IPromise<KeyValuePair<Guid, string>> PromptLogin(ILoginHandler login, OAuth oauth)
        {
            return login.PerformLogin(this, oauth);
        }

        public TestMultiplayerClient(ILoginApi loginApi, string address, int port)
        {
            serverAddress = $"ws://{address}:{port}";
            api = loginApi;
        }

        public void Start()
        {
            client = WebsocketClient.CreateInstance();
            store = MultiplayerStore.Create(this);
            connection = new TestClientConnection(client);
            store.connection.AuthenticateAndConnect().Then(() =>
            {
                connection.SendTestRequests().Then(() =>
                {
                    Debug.Log("[<b>Client</b>] Test successful! Disconnecting...");
                    store.connection.Disconnect();
                    Debug.Log("[<b>Client</b>] Connection test completed.");
                }).Catch(e =>
                {
                    store.connection.Disconnect();
                    Debug.LogError("[<b>Client</b>] Sending a test request failed!");
                    Debug.LogError(e);
                });
            }).Catch(Debug.LogError);
        }

        public void Stop()
        {
            if (client && client.isConnected) client.Disconnect();
        }

        public void SetLoadingStatus(string status)
        {
            Debug.Log($"<b>[Client]</b> Loading status: <b>{status}</b>");
        }
    }
}