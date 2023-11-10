using Essentials;
using Newtonsoft.Json.Linq;

namespace WebsocketMultiplayer
{
    public readonly struct OAuth
    {
        public readonly string key;
        public readonly string url;

        public OAuth(string key, string url)
        {
            this.key = key;
            this.url = url;
        }

        public static bool TryParse(JObject json, out OAuth result)
        {
            var redirectUrl = json.GetString("redirectUrl");
            var sharedSecret = json.GetString("sharedSecret");
            if (redirectUrl == null || sharedSecret == null)
            {
                result = default;
                return false;
            }

            result = new OAuth(key: sharedSecret, url: redirectUrl);
            return true;
        }
    }
}