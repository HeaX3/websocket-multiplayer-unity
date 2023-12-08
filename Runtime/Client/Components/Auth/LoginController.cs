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
        
#if UNITY_WEBGL && !UNITY_EDITOR
        
        [DllImport("__Internal")]
        protected static extern void OpenURL(string url);
        
#else

        protected static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
        
#endif
    }
}