using System.Collections.Generic;
using DataStores;

namespace WebsocketMultiplayer.Client.Modules
{
    public class AnalyticsModule : IDataModule
    {
        private const int MaxFrames = 1000;

        public delegate void FrameEvent(AnalyticsFrame frame);

        public event FrameEvent frameReceived = delegate { };

        private readonly List<AnalyticsFrame> _frames = new();

        public IReadOnlyList<AnalyticsFrame> frames => _frames;

        public void Initialize()
        {
        }

        public void HandleFrame(AnalyticsFrame frame)
        {
            _frames.Add(frame);
            while (_frames.Count > MaxFrames)
            {
                _frames.RemoveAt(0);
            }

            frameReceived(frame);
        }
    }
}