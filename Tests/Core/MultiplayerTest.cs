using System.Collections;
using UnityEngine;
using WebsocketMultiplayer.Tests.Client;
using WebsocketMultiplayer.Tests.Server;

namespace WebsocketMultiplayer.Tests
{
    public class MultiplayerTest : MonoBehaviour
    {
        private TestMultiplayerServer server { get; set; }
        private TestMultiplayerClient client { get; set; }

        private IEnumerator Start()
        {
            var address = "127.0.0.1";
            var port = 8083;
            var dummyApi = new MultiplayerTestApi();

            server = new TestMultiplayerServer(dummyApi, address, port);
            server.Start();

            yield return new WaitForSeconds(1);

            client = new TestMultiplayerClient(dummyApi, address, port);
            client.Start();
        }

        private void OnDisable()
        {
            if (client != null) client.Stop();
            if (server != null) server.Stop();
        }
    }
}