using System;

namespace IntegrationPatterns.Aggregator
{
    public class SegmentedMessage
    {
        public Guid ParentId { get; set; }
        public int CurrentSegment { get; set; }
        public object Payload { get; set; }
    }
}