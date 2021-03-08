using System;
using System.Collections.Generic;

namespace IntegrationPatterns.Aggregator
{
    public class SegmentedMessageParent
    {
        public Guid Id { get; set; }
        public int TotalSegments { get; set; }
        public List<SegmentedMessage> Segments { get; set; }
    }
}