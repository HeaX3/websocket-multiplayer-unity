using System;
using System.Collections.Generic;
using MultiplayerProtocol;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Tests.Client
{
    public class TestClientConnection : ClientConnection
    {
        public TestClientConnection(INetworkEndpoint endpoint) : base(endpoint)
        {
        }

        protected override IEnumerable<INetworkMessageListener> handlers
        {
            get
            {
                foreach (var h in base.handlers) yield return h;
                yield return new TestResponseHandler();
            }
        }

        public IPromise SendTestRequests()
        {
            var requests = new List<Func<IPromise>>();
            var success = 0;
            var fail = 0;
            for (var i = 0; i < 100; i++)
            {
                var index = i;
                requests.Add(() =>
                {
                    return new Promise((resolve, reject) =>
                    {
                        SendRequest(new RequestSpamTestResultMessage()).ThenSuccess(() =>
                        {
                            Debug.Log(index + " success");
                            success++;
                        }).Then(resolve).Catch(e =>
                        {
                            Debug.LogError(index + " error:");
                            Debug.LogError(e);
                            fail++;
                            reject(e);
                        });
                    });
                });
            }
            return Promise.Sequence(requests).Then(() =>
            {
                Debug.Log("Success: " + success + ", Fail: " + fail);
            });
        }
    }
}