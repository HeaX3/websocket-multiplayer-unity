using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WebsocketMultiplayer.Server.Analytics
{
    public class AnalyticsController : MonoBehaviour
    {
        private const int FramerateIntervalSeconds = 10;

        private AnalyticsModule module { get; set; }

        private float _time;
        private readonly List<int> _pastSeconds = new();
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
            _pastSeconds.Add(_frames);
            while (_pastSeconds.Count > FramerateIntervalSeconds) _pastSeconds.RemoveAt(0);
            module.Publish((float)_pastSeconds.Average());
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