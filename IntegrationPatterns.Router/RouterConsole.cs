using IntegrationPatterns.Common;
using IntegrationPatterns.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Router
{
	class RouterConsole
	{
		private static readonly MessageQueue ROUTER_QUEUE = new MessageQueue(@".\private$\router");
		private static readonly MessageQueue CLIENT_A = new MessageQueue(@".\private$\a");
		private static readonly MessageQueue CLIENT_B = new MessageQueue(@".\private$\b");
		private static readonly MessageQueue CLIENT_C = new MessageQueue(@".\private$\c");
		private static readonly IDictionary<string, MessageQueue> CLIENTS = new Dictionary<string, MessageQueue>
		{
			{ CLIENT_A.Path, CLIENT_A },
			{ CLIENT_B.Path, CLIENT_B },
			{ CLIENT_C.Path, CLIENT_C }
		};

		static void Main(string[] args)
		{
			VerifyQueuesExistAndEmpty();

			var router = new Router(ROUTER_QUEUE, CLIENTS);
			QueueReader.BeginReceive(CLIENT_A, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_B, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_C, WriteToConsole);
			router.BeginReceive();

			var messages = new List<RoutedMessage>
			{
				new RoutedMessage { Sender = CLIENT_A.Path, Destination = CLIENT_B.Path, Body = "Message from A to B" },
				new RoutedMessage { Sender = CLIENT_A.Path, Destination = CLIENT_C.Path, Body = "Message from A to C" },
				new RoutedMessage { Sender = CLIENT_B.Path, Destination = CLIENT_A.Path, Body = "Message from B to A" },
				new RoutedMessage { Sender = CLIENT_B.Path, Destination = CLIENT_C.Path, Body = "Message from B to C" },
				new RoutedMessage { Sender = CLIENT_C.Path, Destination = CLIENT_A.Path, Body = "Message from C to A" },
				new RoutedMessage { Sender = CLIENT_C.Path, Destination = CLIENT_B.Path, Body = "Message from C to B" },
			};

			foreach (var m in messages)
			{
				ROUTER_QUEUE.Send(new Message(m));
			}

			Console.ReadLine();

			CleanUp();
		}

		private static void WriteToConsole(Message message)
		{
			message.AttachFormatter(new[] { typeof(RoutedMessage) });
			var routedMessage = (RoutedMessage)message.Body;
			Console.WriteLine(routedMessage.Sender + "->" + routedMessage.Destination + ": " + routedMessage.Body);
		}

		private static void VerifyQueuesExistAndEmpty()
		{
			// Kontroller køerne eksisterer
			ROUTER_QUEUE.VerifyRecreate();
			CLIENT_A.VerifyRecreate();
			CLIENT_B.VerifyRecreate();
			CLIENT_C.VerifyRecreate();

			// Tøm køerne
			ROUTER_QUEUE.Purge();
			CLIENT_A.Purge();
			CLIENT_B.Purge();
			CLIENT_C.Purge();
		}

		private static void CleanUp()
		{
			// Hvorfor udløser dette en exception???????????
			ROUTER_QUEUE.Delete();
			CLIENT_A.Delete();
			CLIENT_B.Delete();
			CLIENT_C.Delete();
		}
	}
}
