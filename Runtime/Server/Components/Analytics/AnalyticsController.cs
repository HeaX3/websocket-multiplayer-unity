using UnityEngine;

namespace WebsocketMultiplayer.Server.Analytics
{
    public class AnalyticsController : MonoBehaviour
    {
        private AnalyticsModule module { get; set; }

        private float _time;
        private int _frames;

        private AnalyticsController Initialize(AnalyticsModule module)
        {
            this.module = module;
            return this;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            _frames++;
            if (_time < 1) return;
            module.Publish(_frames);
            _time = 0;
            _frames = 0;
        }

        internal static AnalyticsController Create(AnalyticsModule module)
        {
            var gameObject = new GameObject("Analytics");
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<AnalyticsController>().Initialize(module);
        }
    }
}