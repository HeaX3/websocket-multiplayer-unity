using System;
using System.Collections.Generic;
using RSG;

namespace WebsocketMultiplayer.Client
{
    public interface ILoginHandler
    {
        IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client);
        void Cancel(ILoginClient client);
    }
}