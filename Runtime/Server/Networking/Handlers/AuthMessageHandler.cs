﻿using System;
using MultiplayerProtocol;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Server
{
    public class AuthMessageHandler : IAsyncNetworkRequestHandler<AuthMessage>
    {
        private IMultiplayerServer server { get; }
        private IClientConnection connection { get; }
        public bool debug { get; set; }

        public uint maxTimeoutMs => 30000;

        public AuthMessageHandler(IMultiplayerServer server, IClientConnection connection, bool debug = false)
        {
            this.server = server;
            this.connection = connection;
            this.debug = debug;
        }

        public IPromise<IRequestResponse> Handle(AuthMessage message)
        {
            return new Promise<IRequestResponse>((resolve, reject) =>
            {
                if (debug) Debug.Log("Authenticate with the API");
                Authenticate(message.userId, message.jwt).Then(result =>
                {
                    if (debug) Debug.Log("Authenticated with the API");
                    connection.userId = result.userId;
                    server.joinHandler.HandleUserJoin(connection, result.user).Then(joinResult =>
                    {
                        var response = new RequestResponse(new AuthResultValue(result.jwt, result.userId))
                        {
                            preResponse = joinResult?.preResponse,
                            postResponse = joinResult?.postResponse
                        };
                        if (debug)
                        {
                            Debug.Log(
                                "Pre: " + response.preResponse?.value?.Length + "\n" +
                                "Post: " + response.postResponse?.value?.Length
                            );
                        }

                        resolve(response);
                    }).Catch(e =>
                    {
                        if (debug || e is not AuthenticationException)
                        {
                            Debug.LogError("API Authentication failed:");
                            Debug.LogError(e);
                        }

                        reject(e);
                    });
                }).Catch(e =>
                {
                    if (debug || e is not AuthenticationException)
                    {
                        Debug.LogError("API Authentication failed:");
                        Debug.LogError(e);
                    }

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

        private IPromise<AuthenticationResultDto> Authenticate(Guid userId, string secret)
        {
            if (server.login.api is IJwtAuthHandler jwtAuthHandler)
            {
                return jwtAuthHandler.Authenticate(secret);
            }

            if (server.login.api is ITokenAuthHandler tokenAuthHandler)
            {
                return tokenAuthHandler.Authenticate(userId, secret);
            }

            throw new NotImplementedException(
                "Your Login API must implement either IJwtAuthHandler or ITokenAuthHandler"
            );
        }
    }
}