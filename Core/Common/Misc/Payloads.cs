using System;

namespace Hermit.Common
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