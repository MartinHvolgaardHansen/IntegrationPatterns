using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;

namespace IntegrationPatterns.Aggregator
{
    public static class AggregatorExtension
    {
        public static Message Aggregate(this Message parent, IEnumerable<MessageSegment> segments)
		{
			MessageSegmentHeader header;
			try
			{
				header = (MessageSegmentHeader)parent.Body;
			}
			catch (InvalidCastException ex)
			{
				throw new ArgumentException("Message must contain MessageSegmentHeader", ex);
			}
			if (Validate(header, segments))
			{
				header.Segments = segments;
				return parent;
			}
			else
			{
				throw new InvalidOperationException("Segments are incomplete");
			}
		}

		private static bool Validate(MessageSegmentHeader header, IEnumerable<MessageSegment> segments)
		{
			var countValidated = segments.Count().Equals(header.TotalSegments);
			var contentValidated = true;
			var sequenceValidated = true;
			for (int i = 0; i < header.TotalSegments; i++)
			{
				var segment = segments.ElementAt(i);
				if (!segment.MessageId.Equals(header.MessageId))
					contentValidated = false;
				if (!segment.CurrentSegment.Equals(i + 1))
					sequenceValidated = false;
			}
			return countValidated && contentValidated && sequenceValidated;
		}
    }
}