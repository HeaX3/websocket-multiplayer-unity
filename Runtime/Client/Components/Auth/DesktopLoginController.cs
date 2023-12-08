using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;

namespace WebsocketMultiplayer.Client
{
    public class DesktopLoginController : LoginController
    {
        private static DesktopLoginController _instance;

        public static DesktopLoginController Instance
        {
            get
            {
                if (!_instance) CreateInstance();
                return _instance;
            }
        }

        private bool _cancelled;
        private Action<Exception> reject;

        private static void CreateInstance()
        {
            _instance = new GameObject("Desktop Auth").AddComponent<DesktopLoginController>();
            _instance.hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            DontDestroyOnLoad(_instance);
        }

        public override IPromise<KeyValuePair<Guid, string>> PerformLogin(ILoginClient client, OAuth auth)
        {
            return new Promise<KeyValuePair<Guid, string>>((resolve, reject) =>
            {
                if (this.reject != null)
                {
                    reject(new Exception("Cannot start auth again while it is already in progress"));
                    return;
                }

                if (!string.IsNullOrWhiteSpace(auth.url))
                {
                    OpenURL(auth.url);
                }
                StartCoroutine(PingRoutine(client, auth.key, resolve, reject));
            });
        }

        public override void Cancel(ILoginClient client)
        {
            _cancelled = true;
            if (reject != null)
            {
                reject(new AuthenticationCancelledException());
            }
            else
            {
                Debug.LogWarning("Reject handler was null");
            }

            reject = default;
        }

        private IEnumerator PingRoutine(ILoginClient client, string key, Action<KeyValuePair<Guid, string>> resolve,
            Action<Exception> reject)
        {
            this.reject = reject;
            _cancelled = false;
            var remaining = 300;
            var done = false;
            while (remaining > 0 && !done)
            {
                if (_cancelled)
                {
                    yield break;
                }

                client.api.PingOAuth(key).Then(response =>
                {
                    if (!response.done) return;
                    done = true;
                    if (this.reject == default) return;
                    this.reject = default;
                    resolve(new KeyValuePair<Guid, string>(response.userId, response.jwt));
                }).Catch(e =>
                {
                    done = true;
                    if (this.reject == default) return;
                    this.reject = default;
                    reject(e);
                });
                yield return new WaitForSeconds(5);
                remaining -= 5;
            }

            if (done) yield break;
            this.reject = default;
            reject(new TimeoutException());
        }
    }
}