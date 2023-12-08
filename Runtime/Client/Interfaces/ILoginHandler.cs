using System;
using System.Collections.Generic;
using RSG;

namespace WebsocketMultiplayer.Client
{
    public interface ILoginHandler
    {
        IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client, OAuth auth);
        void Cancel(ILoginClient client);
    }
}