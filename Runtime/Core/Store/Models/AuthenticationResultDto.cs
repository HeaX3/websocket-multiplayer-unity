using System;
using Essentials;
using Newtonsoft.Json.Linq;

namespace WebsocketMultiplayer
{
    public readonly struct AuthenticationResultDto
    {
        public readonly string jwt;
        public readonly Guid userId;
        public readonly JObject user;

        public AuthenticationResultDto(string jwt, JObject user)
        {
            this.jwt = jwt;
            userId = user.GetGuid("id");
            this.user = user;
        }

        public static bool TryParse(JObject json, out AuthenticationResultDto result)
        {
            var jwt = json.GetString("jwt");
            var user = json.GetSection("user");
            if (jwt == null)
            {
                result = default;
                return false;
            }

            result = new AuthenticationResultDto(jwt, user);
            return true;
        }
    }
}