using System;
using System.Collections.Generic;
using RSG;
using WebsocketMultiplayer.Client;

namespace WebsocketMultiplayer.Tests.Client
{
    public class DummyLoginHandler : ILoginHandler
    {
        public IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client, OAuth _)
        {
            return new Promise<KeyValuePair<Guid, string>>((resolve, reject) =>
            {
                client.api.PingOAuth(client.platform).Then(result =>
                {
                    resolve(new KeyValuePair<Guid, string>(result.userId, result.jwt));
                }).Catch(reject);
            });
        }

        public void Cancel(ILoginClient client)
        {
            // Not needed
        }
    }
}