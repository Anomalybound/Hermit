using System;

namespace Hermit
{
    public class Payloads
    {
        public static readonly Payloads Empty = new Payloads();

        public DateTime SentTime { get; set; }

        protected Payloads()
        {
            SentTime = DateTime.Now;
        }
    }
}