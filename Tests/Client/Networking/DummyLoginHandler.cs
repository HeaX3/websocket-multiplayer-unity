using System;
using System.Collections.Generic;
using RSG;
using WebsocketMultiplayer.Client;

namespace WebsocketMultiplayer.Tests.Client
{
    public class DummyLoginHandler : ILoginHandler
    {
        public IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client)
        {
            return new Promise<KeyValuePair<Guid, string>>((resolve, reject) =>
            {
                client.api.PingOAuth(client.platform).Then(oauth =>
                {
                    resolve(new KeyValuePair<Guid, string>(oauth.userId, oauth.jwt));
                }).Catch(reject);
            });
        }

        public void Cancel(ILoginClient client)
        {
            // Not needed
        }
    }
}