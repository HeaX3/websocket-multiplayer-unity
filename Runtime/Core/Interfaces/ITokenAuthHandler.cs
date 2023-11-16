using System;
using RSG;

namespace WebsocketMultiplayer
{
    public interface ITokenAuthHandler
    {
        public IPromise<AuthenticationResultDto> Authenticate(Guid userId, string secret);
    }
}