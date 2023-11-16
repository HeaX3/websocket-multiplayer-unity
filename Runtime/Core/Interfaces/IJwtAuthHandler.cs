using RSG;

namespace WebsocketMultiplayer
{
    public interface IJwtAuthHandler
    {
        public IPromise<AuthenticationResultDto> Authenticate(string jwt);
    }
}