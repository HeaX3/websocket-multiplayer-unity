using System;
using DataStores;
using Essentials;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace WebsocketMultiplayer.Client.Modules
{
    public class AuthModule : IDataModule
    {
        private IMultiplayerClient client { get; }
        
        private const string file = "login.json";

        public delegate void StringEvent(string value);

        public event StringEvent secretChanged = delegate { };

        public Guid userId;
        public string _secret;

        public string secret
        {
            get => _secret;
            set => ApplySecret(value);
        }
        
        public AuthModule(IMultiplayerClient client)
        {
            this.client = client;
        }

        public void Initialize()
        {
            
        }

        public void Reset()
        {
            userId = default;
            secret = default;
            SaveCredentials();
        }

        public void FetchCredentials()
        {
            var json = client.files.ReadJsonEncrypted(file);
            if (json == null)
            {
                return;
            }

            userId = json.GetGuid("userId");
            secret = json.GetString("secret");
        }

        public void SaveCredentials()
        {
            var json = new JObject().Set("userId", userId).Set("secret", secret);
            try
            {
                client.files.WriteEncrypted(file, json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void ApplySecret(string value)
        {
            _secret = value;
            secretChanged(value);
        }
    }
}