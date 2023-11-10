using System;
using HttpApis;
using Newtonsoft.Json.Linq;
using RSG;
using UnityEngine.Networking;

namespace WebsocketMultiplayer
{
    public class LoginApi : HttpApi, ILoginApi
    {
        public IPromise<OAuth> StartOAuth(string client)
        {
            return new Promise<OAuth>((resolve, reject) =>
            {
                Post($"/oauth", new JObject() { ["userAgent"] = client, ["entryPoint"] = "/" }).Then(response =>
                {
                    OAuth oauth;
                    try
                    {
                        var json = response.json();
                        if (!OAuth.TryParse(json, out oauth))
                        {
                            reject(new Exception("Invalid response received:\n" + response));
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        reject(e);
                        return;
                    }

                    resolve(oauth);
                }).Catch(reject);
            });
        }

        public IPromise<OAuthPing> PingOAuth(string key)
        {
            return new Promise<OAuthPing>((resolve, reject) =>
            {
                Get($"/oauth/ping/{key}").Then(response =>
                {
                    OAuthPing oauth;
                    try
                    {
                        var json = response.json();
                        if (!OAuthPing.TryParse(json, out oauth))
                        {
                            reject(new Exception("Invalid response received:\n" + response));
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        reject(e);
                        return;
                    }

                    resolve(oauth);
                });
            });
        }

        public IPromise<AuthenticationResultDto> Authenticate(string jwt)
        {
            return new Promise<AuthenticationResultDto>((resolve, reject) =>
            {
                var www = new UnityWebRequest(BuildUrl("/oauth/authenticate"), "POST");
                www.SetRequestHeader("Authorization", $"Bearer {jwt}");
                www.SetRequestHeader("Accept", "application/json");

                www.downloadHandler = new DownloadHandlerBuffer();
                Perform(www).Then(result =>
                {
                    try
                    {
                        var json = JObject.Parse(result.downloadHandler.text);
                        if (!AuthenticationResultDto.TryParse(json, out var authResult))
                        {
                            reject(new Exception("Authentication returned unexpected result"));
                            return;
                        }

                        resolve(authResult);
                    }
                    catch (Exception e)
                    {
                        reject(e);
                    }
                }).Catch(reject);
            });
        }
    }
}