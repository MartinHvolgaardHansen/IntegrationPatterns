using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Router
{
	public class Router
	{
		private readonly MessageQueue routerQueue;
		private readonly IDictionary<string, MessageQueue> clientQueues;

		public Router(
			MessageQueue routerQueue,
			IDictionary<string, MessageQueue> clientQueues)
		{
			this.routerQueue = routerQueue;
			this.clientQueues = clientQueues;
		}

		public void BeginReceive()
		{
			QueueReader.BeginReceive(routerQueue, OnMessageReceived);
		}

		private void OnMessageReceived(Message message)
		{
			RouteToDestination(message);
			routerQueue.BeginReceive();
		}

		private void RouteToDestination(Message message)
		{
			message.AttachFormatter(new[] { typeof(RoutedMessage) });
			var routedMessage = (RoutedMessage)message.Body;
			var clientQueue = this.clientQueues[routedMessage.Destination];
			clientQueue.Send(message);
		}
	}
}