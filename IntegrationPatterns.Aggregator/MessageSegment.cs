using System;

namespace IntegrationPatterns.Aggregator
{
    public class MessageSegment
    {
        public string MessageId { get; set; }
        public int CurrentSegment { get; set; }
        public object Payload { get; set; }
    }
}