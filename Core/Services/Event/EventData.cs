using System;

namespace Hermit
{
    public class EventData
    {
        public static readonly EventData Empty = new EventData();

        public DateTime SentTime { get; set; }

        protected EventData()
        {
            SentTime = DateTime.Now;
        }
    }
}