using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.RequestReply
{
	static class QueueReader
	{
		public static void BeginRead(MessageQueue queue, Action<Message> onReceiveCompleted)
		{
			queue.ReceiveCompleted += (s, e) => OnMessageReceived(s, e, onReceiveCompleted);
			// Begynd at lytte på indkomne beskeder
			queue.BeginReceive();
		}

		private static void OnMessageReceived(object sender, ReceiveCompletedEventArgs e, Action<Message> onReceiveCompleted)
		{
			var queue = (MessageQueue)sender;
			// Udlæs beskeden fra køen
			var message = queue.EndReceive(e.AsyncResult);
			onReceiveCompleted(message);
			// Fortsæt med at lytte på indkomne beskeder
			queue.BeginReceive();
		}
	}
}
