using System;
using System.Collections.Generic;

namespace IntegrationPatterns.Aggregator
{
    public class MessageSegmentHeader
    {
        public string MessageId { get; set; }
        public int TotalSegments { get; set; }
        public IEnumerable<MessageSegment> Segments { get; set; }
    }
}