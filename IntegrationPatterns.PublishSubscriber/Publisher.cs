using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.PublishSubscriber
{
	public class Publisher
	{
		private readonly MessageQueue publishQueue;
		private readonly List<MessageQueue> clientQueues;

		public Publisher(
			MessageQueue publishQueue,
			List<MessageQueue> clientQueues)
		{
			this.publishQueue = publishQueue;
			this.clientQueues = clientQueues;
		}

		public void BeginReceive()
		{
			QueueReader.BeginReceive(publishQueue, OnMessageReceived);
		}

		private void OnMessageReceived(Message message)
		{
			Publish(message);
			publishQueue.BeginReceive();
		}

		private void Publish(Message message)
		{
			foreach (var c in clientQueues)
			{
				c.Send(message);
			}
		}
	}
}
