using MultiplayerProtocol;

namespace WebsocketMultiplayer.Tests.Server.Handlers
{
    public class RequestSpamTestHandler : INetworkRequestHandler<RequestSpamTestResultMessage>
    {
        public IRequestResponse Handle(RequestSpamTestResultMessage message)
        {
            return RequestResponse.Ok();
        }
    }
}