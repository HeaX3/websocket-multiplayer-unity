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
        public IMultiplayerConnection connection { get; private set; }
        public ILoginClient login => this;
        public ILoginApi api { get; }
        public string platform => "test";
        public ILoginHandler authentication { get; } = new DummyLoginHandler();

        public IPromise<KeyValuePair<Guid, string>> PromptLogin(ILoginHandler login)
        {
            return login.PerformLogin(this);
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
            connection = new ClientConnection(client);
            store.connection.AuthenticateAndConnect().Then(() =>
            {
                Debug.Log("<b>[Client]</b> Test successful! Disconnecting...");
                store.connection.Disconnect();
                Debug.Log("<b>[Client]</b> Connection test completed.");
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