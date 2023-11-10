using MultiplayerProtocol;
using RSG;

namespace WebsocketMultiplayer.Server
{
    public class AuthMessageHandler : IAsyncNetworkRequestHandler<AuthMessage>
    {
        private IMultiplayerServer server { get; }
        private IClientConnection connection { get; }

        public AuthMessageHandler(IMultiplayerServer server, IClientConnection connection)
        {
            this.server = server;
            this.connection = connection;
        }

        public IPromise<IRequestResponse> Handle(AuthMessage message)
        {
            return new Promise<IRequestResponse>((resolve, reject) =>
            {
                server.login.api.Authenticate(message.jwt.value).Then(result =>
                {
                    connection.userId = result.userId;
                    server.joinHandler.HandleUserJoin(connection, result.user).Then(joinResult =>
                    {
                        var response = new AuthResultMessage(result.jwt, result.userId, joinResult);
                        resolve(response);
                    }).Catch(reject);
                }).Catch(e =>
                {
                    if (e is AuthenticationException ||
                        (e.Message?.StartsWith("401: HTTP/1.1 401 Unauthorized") ?? false))
                    {
                        resolve(RequestResponse.Forbidden("Invalid credentials"));
                        return;
                    }

                    reject(e);
                });
            });
        }
    }
}