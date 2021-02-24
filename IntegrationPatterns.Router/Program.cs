using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Router
{
	class Program
	{
		private static readonly MessageQueue ROUTER_QUEUE = new MessageQueue(@".\private$\router") { Label = "router" };
		private static readonly MessageQueue CLIENT_A = new MessageQueue(@".\private$\a") { Label = "a" };
		private static readonly MessageQueue CLIENT_B = new MessageQueue(@".\private$\b") { Label = "b" };
		private static readonly MessageQueue CLIENT_C = new MessageQueue(@".\private$\c") { Label = "c" };
		private static IDictionary<string, MessageQueue> CLIENTS = new Dictionary<string, MessageQueue>
			{
				{ CLIENT_A.Label, CLIENT_A
	},
				{ CLIENT_B.Label, CLIENT_B
},
				{ CLIENT_C.Label, CLIENT_C }
			};

static async Task Main(string[] args)
		{
			VerifyQueuesExistAndEmpty();

			var router = new Router(ROUTER_QUEUE, CLIENTS);
			QueueReader.BeginReceive(CLIENT_A, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_B, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_C, WriteToConsole);
			await router.BeginReceive();

			var messages = new List<RoutedMessage>
			{
				new RoutedMessage { Sender = CLIENT_A.Label, Destination = CLIENT_B.Label, Body = "Message from A to B" },
				new RoutedMessage { Sender = CLIENT_A.Label, Destination = CLIENT_C.Label, Body = "Message from A to C" },
				new RoutedMessage { Sender = CLIENT_B.Label, Destination = CLIENT_A.Label, Body = "Message from B to A" },
				new RoutedMessage { Sender = CLIENT_B.Label, Destination = CLIENT_C.Label, Body = "Message from B to C" },
				new RoutedMessage { Sender = CLIENT_C.Label, Destination = CLIENT_A.Label, Body = "Message from C to A" },
				new RoutedMessage { Sender = CLIENT_C.Label, Destination = CLIENT_B.Label, Body = "Message from C to B" },
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
			CLIENT_A.Verify();
			CLIENT_B.Verify();
			CLIENT_C.Verify();

			// Tøm køerne
			ROUTER_QUEUE.Purge();
			CLIENT_A.Purge();
			CLIENT_B.Purge();
			CLIENT_C.Purge();
		}

		private static void CleanUp()
		{
			ROUTER_QUEUE.Delete();
			CLIENT_A.Delete();
			CLIENT_B.Delete();
			CLIENT_C.Delete();
		}
	}
}
