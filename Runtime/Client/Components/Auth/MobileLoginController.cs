using System;
using System.Collections.Generic;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Client
{
    public class MobileLoginController : LoginController
    {
        private static MobileLoginController _instance;

        public static MobileLoginController Instance
        {
            get
            {
                if (!_instance) CreateInstance();
                return _instance;
            }
        }

        private static void CreateInstance()
        {
            _instance = new GameObject("Mobile Auth").AddComponent<MobileLoginController>();
            _instance.hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            DontDestroyOnLoad(_instance);
        }

        public override IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client, OAuth auth)
        {
            // TODO Do some kind of mobile specific wizardry to control the login flow in an in-app browser
            throw new NotImplementedException();
        }

        public override void Cancel(ILoginClient client)
        {
            // TODO Close the in-app browser I guess?
            throw new NotImplementedException();
        }
    }
}