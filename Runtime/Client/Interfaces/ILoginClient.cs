using System;
using System.Collections.Generic;
using RSG;

namespace WebsocketMultiplayer.Client
{
    public interface ILoginClient
    {
        ILoginApi api { get; }
        string platform { get; }
        ILoginHandler authentication { get; }
        IPromise<KeyValuePair<Guid, string>> PromptLogin(ILoginHandler login);
        IPromise<KeyValuePair<Guid, string>> PromptLogin() => PromptLogin(authentication);
    }
}