using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.RequestReply
{
	class RequestReplyConsole
	{
		// Opret køerne som bliver brugt til at sende og modtage
		private static readonly MessageQueue TO_REQUESTER = new MessageQueue(@".\private$\requester");
		private static readonly MessageQueue TO_REPLIER = new MessageQueue(@".\private$\replier");

		static void Main(string[] args)
		{
			// Kontroller at køerne er klar til at sende og modtage
			VerifyQueuesExistAndEmpty();

			// Opret beskederne (requests) som skal sendes
			var messageToReplier1 = new Message(new Request("first"));
			var messageToReplier2 = new Message(new Request("second"));
			var messageToReplier3 = new Message(new Request("third"));

			// Start køerne op og sæt dem i "BeginReceive" (Se QueueReader klassen)
			QueueReader.BeginReceive(TO_REPLIER, OnRequestReceived);
			QueueReader.BeginReceive(TO_REQUESTER, OnReplyReceived);

			// Send "Requests" til "Replier" køen
			TO_REPLIER.Send(messageToReplier1);
			TO_REPLIER.Send(messageToReplier2);
			TO_REPLIER.Send(messageToReplier3);

			// Forhindr konsollen i at lukke
			Console.ReadLine();

			CleanUp();
		}

		// Køres når "Replier" køen modtager en "Request"
		private static void OnRequestReceived(Message message)
		{
			// Opretter en "Formatter" og fortæller at den skal kunne arbejde med en "Request"
			message.Formatter = new XmlMessageFormatter(new[] { typeof(Request) });
			if (message.Body.GetType().Equals(typeof(Request)))
			{
				var request = (Request)message.Body;

				// Simuler data bliver hentet fra en database, med en primary key (ID)
				var data = Data.Get(request.Id);

				// Opret og send "Reply"
				var reply = new Reply(request.Id, data);
				Console.WriteLine("Replier received your request with ID: " + request.Id + ". Replying with data: " + reply.Data);
				var messageToRequester = new Message(reply);
				TO_REQUESTER.Send(messageToRequester);
			}
		}

		// Køres når "Requester" køen modtager en "Reply"
		private static void OnReplyReceived(Message message)
		{
			// Opretter en "Formatter" og fortæller at den skal kunne arbejde med en "Reply"
			message.Formatter = new XmlMessageFormatter(new[] { typeof(Reply) });
			if (message.Body.GetType().Equals(typeof(Reply)))
			{
				var reply = (Reply)message.Body;
				Console.WriteLine("Requester received your reply for request ID: " + reply.RequestId + ", containing data: " + reply.Data);
			}
		}

		private static void VerifyQueuesExistAndEmpty()
		{
			// Kontroller køerne eksisterer
			TO_REQUESTER.Verify();
			TO_REPLIER.Verify();

			// Tøm køerne
			TO_REQUESTER.Purge();
			TO_REPLIER.Purge();
		}

		private static void CleanUp()
		{
			// Slet køerne
			TO_REQUESTER.Delete();
			TO_REPLIER.Delete();
		}
	}
}
