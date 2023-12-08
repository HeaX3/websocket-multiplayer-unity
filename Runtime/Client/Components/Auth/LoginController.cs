using System;
using System.Collections.Generic;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Client
{
    public abstract class LoginController : MonoBehaviour, ILoginHandler
    {
        public abstract IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client, OAuth auth);
        public abstract void Cancel(ILoginClient client);
    }
}