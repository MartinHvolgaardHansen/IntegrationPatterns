using System;
using System.Collections;
using System.Collections.Generic;

namespace IntegrationPatterns.Aggregator
{
    public class Aggregator
    {
        private readonly MessageQueue _unsequencedQueue;
        private readonly MessageQueue _sequencedQueue;
        private readonly IDictionary<Guid, List<SequenceMessage>> _unorderedSequences;

        public Resequencer(MessageQueue unsequencedQueue, MessageQueue sequencedQueue)
        {
            _unsequencedQueue = unsequencedQueue;
            _sequencedQueue = sequencedQueue;
            _unorderedSequences = new Dictionary<Guid, List<SequenceMessage>>();
        }

        public void BeginReceive()
        {
            QueueReader.BeginReceive(_unsequencedQueue, OnMessageReceived);
        }

        private void OnMessageReceived(Message message)
        {
            Resequence(message);
            _unsequencedQueue.BeginReceive();
        }

        private void Resequence(Message message)
        {
            message.AttachFormatter(new[] { typeof(SequenceMessage) });
            var sequenceMessage = (SequenceMessage)message.Body;
            var id = sequenceMessage.Id;

            if (_unorderedSequences.ContainsKey(id) && !_unorderedSequences[id]
                .Any(sm => sm.Sequence.Equals(sequenceMessage.Sequence)))
                _unorderedSequences[id].Add(sequenceMessage);
            else
                _unorderedSequences.Add(id, new List<SequenceMessage> { sequenceMessage });

            if (_unorderedSequences[id].Count.Equals(sequenceMessage.SequenceMax))
                SendMessageSequence(_unorderedSequences[id].OrderBy(sm => sm.Id).ThenBy(sm => sm.Sequence));
        }
    }
}