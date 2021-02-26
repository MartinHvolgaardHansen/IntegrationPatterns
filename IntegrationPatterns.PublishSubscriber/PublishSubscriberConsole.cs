using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.PublishSubscriber
{
	class PublishSubscriberConsole
	{
		private static readonly MessageQueue PUBLISH_QUEUE = new MessageQueue(@".\private$\publish");
		private static readonly MessageQueue CLIENT_A = new MessageQueue(@".\private$\a");
		private static readonly MessageQueue CLIENT_B = new MessageQueue(@".\private$\b");
		private static readonly MessageQueue CLIENT_C = new MessageQueue(@".\private$\c");
		private static readonly List<MessageQueue> CLIENTS = new List<MessageQueue>
		{
			CLIENT_A,
			CLIENT_B,
			CLIENT_C
		};

		static void Main(string[] args)
		{
			VerifyQueuesExistAndEmpty();

			var publisher = new Publisher(PUBLISH_QUEUE, CLIENTS);
			QueueReader.BeginReceive(CLIENT_A, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_B, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_C, WriteToConsole);
			publisher.BeginReceive();

			PUBLISH_QUEUE.Send(new Message(new PublishedMessage { Body = "Published information" }));

			Console.ReadLine();

			CleanUp();
		}

		private static void WriteToConsole(Message message)
		{
			message.AttachFormatter(new[] { typeof(PublishedMessage) });
			var publishedMessage = (PublishedMessage)message.Body;
			Console.WriteLine(publishedMessage.Body);
		}

		private static void VerifyQueuesExistAndEmpty()
		{
			// Kontroller køerne eksisterer
			PUBLISH_QUEUE.VerifyRecreate();
			CLIENT_A.VerifyRecreate();
			CLIENT_B.VerifyRecreate();
			CLIENT_C.VerifyRecreate();

			// Tøm køerne
			PUBLISH_QUEUE.Purge();
			CLIENT_A.Purge();
			CLIENT_B.Purge();
			CLIENT_C.Purge();
		}

		private static void CleanUp()
		{
			PUBLISH_QUEUE.Delete();
			CLIENT_A.Delete();
			CLIENT_B.Delete();
			CLIENT_C.Delete();
		}
	}
}
