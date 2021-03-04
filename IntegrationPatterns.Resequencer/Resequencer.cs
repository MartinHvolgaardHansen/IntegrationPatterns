using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Resequencer
{
	class Resequencer
	{
		private MessageQueue unsequencedQueue;
		private MessageQueue sequencedQueue;
		private IDictionary<Guid, List<SequenceMessage>> unorderedSequences;

		public Resequencer(MessageQueue unsequencedQueue, MessageQueue sequencedQueue)
		{
			this.unsequencedQueue = unsequencedQueue;
			this.sequencedQueue = sequencedQueue;
			this.unorderedSequences = new Dictionary<Guid, List<SequenceMessage>>();
		}

		public void BeginReceive()
		{
			QueueReader.BeginReceive(unsequencedQueue, OnMessageReceived);
		}

		private void OnMessageReceived(Message message)
		{
			Resequence(message);
			unsequencedQueue.BeginReceive();
		}

		private void Resequence(Message message)
		{
			message.AttachFormatter(new[] { typeof(SequenceMessage) });
			var sequenceMessage = (SequenceMessage)message.Body;
			var id = sequenceMessage.Id;
			var hasSequence = this.unorderedSequences.ContainsKey(id);
			var hasMessage = false;
			if (hasSequence)
				hasMessage = this.unorderedSequences[id]
					.Any(sm => sm.Sequence.Equals(sequenceMessage.Sequence));

			if (hasSequence && !hasMessage)
			{
				this.unorderedSequences[id].Add(sequenceMessage);
			}
			else
			{
				this.unorderedSequences.Add(id, new List<SequenceMessage> { sequenceMessage });
			}

			if (this.unorderedSequences[id].Count.Equals(sequenceMessage.SequenceMax))
				SendMessageSequence(this.unorderedSequences[id].OrderBy(sm => sm.Sequence));
		}

		private void SendMessageSequence(IEnumerable<SequenceMessage> sequencedMessages)
		{
			foreach (var sm in sequencedMessages)
			{
				this.sequencedQueue.Send(new Message(sm));
			}
		}
	}
}
