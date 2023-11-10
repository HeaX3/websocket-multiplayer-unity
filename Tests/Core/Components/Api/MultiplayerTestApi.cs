using System;
using Newtonsoft.Json.Linq;
using RSG;

namespace WebsocketMultiplayer.Tests
{
    public class MultiplayerTestApi : ILoginApi
    {
        private readonly Guid userId = Guid.NewGuid();

        public IPromise<OAuth> StartOAuth(string client)
        {
            return Promise<OAuth>.Resolved(new OAuth(key: "", url: ""));
        }

        public IPromise<OAuthPing> PingOAuth(string key)
        {
            return Promise<OAuthPing>.Resolved(new OAuthPing(true, userId, "supersecret"));
        }

        public IPromise<AuthenticationResultDto> Authenticate(string jwt)
        {
            return Promise<AuthenticationResultDto>.Resolved(new AuthenticationResultDto(jwt, new JObject
            {
                ["id"] = userId,
            }));
        }
    }
}