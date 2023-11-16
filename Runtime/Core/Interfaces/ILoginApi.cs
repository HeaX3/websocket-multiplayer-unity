using RSG;

namespace WebsocketMultiplayer
{
    public interface ILoginApi
    {
        public IPromise<OAuth> StartOAuth(string client);
        public IPromise<OAuthPing> PingOAuth(string key);
    }
}