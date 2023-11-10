using System;
using Essentials;
using Newtonsoft.Json.Linq;

namespace WebsocketMultiplayer
{
    public readonly struct OAuthPing
    {
        public readonly bool done;
        public readonly Guid userId;
        public readonly string jwt;

        public OAuthPing(bool done, Guid userId, string jwt)
        {
            this.done = done;
            this.userId = userId;
            this.jwt = jwt;
        }

        public static bool TryParse(JObject json, out OAuthPing result)
        {
            var done = json.GetBool("done");
            var userId = json.GetGuid("userId");
            var jwt = json.GetString("secret");

            result = new OAuthPing(
                done,
                userId,
                jwt
            );
            return true;
        }
    }
}