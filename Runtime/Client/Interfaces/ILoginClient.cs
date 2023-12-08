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
        IPromise<KeyValuePair<Guid, string>> PromptLogin(ILoginHandler login, OAuth oauth);

        IPromise<KeyValuePair<Guid, string>> PromptLogin()
        {
            return new Promise<KeyValuePair<Guid, string>>((resolve, reject) =>
            {
                api.StartOAuth(platform).Then(oauth =>
                {
                    PromptLogin(authentication, oauth).Then(resolve).Catch(reject);
                }).Catch(reject);
            });
        }
    }
}