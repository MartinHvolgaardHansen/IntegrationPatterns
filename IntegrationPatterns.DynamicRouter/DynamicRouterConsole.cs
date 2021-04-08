using IntegrationPatterns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.DynamicRouter
{
	class DynamicRouterConsole
	{
		private static readonly MessageQueue ROUTER_QUEUE = new MessageQueue(@".\private$\router");
		private static readonly MessageQueue CLIENT_A = new MessageQueue(@".\private$\a");
		private static readonly MessageQueue CLIENT_B = new MessageQueue(@".\private$\b");
		private static readonly MessageQueue CLIENT_C = new MessageQueue(@".\private$\c");
		private static readonly MessageQueue CLIENT_D = new MessageQueue(@".\private$\d");
		private static readonly MessageQueue CLIENT_E = new MessageQueue(@".\private$\e");
		private static readonly MessageQueue CLIENT_F = new MessageQueue(@".\private$\f");
		private static readonly IDictionary<string, MessageQueue> CLIENTS = new Dictionary<string, MessageQueue>
		{
			{ CLIENT_A.Path, CLIENT_A },
			{ CLIENT_B.Path, CLIENT_B },
			{ CLIENT_C.Path, CLIENT_C },
			{ CLIENT_D.Path, CLIENT_D },
			{ CLIENT_E.Path, CLIENT_E },
			{ CLIENT_F.Path, CLIENT_F }
		};
		private static readonly IDictionary<int, RoutingRule> RULES = new Dictionary<int, RoutingRule>
		{
			{ 1, new RoutingRule { GateNumber = 1, IntervalStart = new DateTime(2021, 4, 7), IntervalEnd = new DateTime(2021, 4, 10), QueueId = CLIENT_A.Path } },
			{ 2, new RoutingRule { GateNumber = 2, IntervalStart = new DateTime(2021, 4, 7), IntervalEnd = new DateTime(2021, 4, 10), QueueId = CLIENT_B.Path } },
			{ 3, new RoutingRule { GateNumber = 3, IntervalStart = new DateTime(2021, 4, 7), IntervalEnd = new DateTime(2021, 4, 10), QueueId = CLIENT_C.Path } },
			{ 5, new RoutingRule { GateNumber = 5, IntervalStart = new DateTime(2021, 4, 7), IntervalEnd = new DateTime(2021, 4, 10), QueueId = CLIENT_D.Path } },
			{ 8, new RoutingRule { GateNumber = 8, IntervalStart = new DateTime(2021, 4, 7), IntervalEnd = new DateTime(2021, 4, 10), QueueId = CLIENT_E.Path } },
			{ 13, new RoutingRule { GateNumber = 13, IntervalStart = new DateTime(2021, 4, 7), IntervalEnd = new DateTime(2021, 4, 10), QueueId = CLIENT_F.Path } }
		};

		static void Main(string[] args)
		{
			VerifyQueuesExistAndEmpty();
			var repo = new RoutingRuleRepository(RULES);
			var router = new DynamicRouter(ROUTER_QUEUE, CLIENTS, repo);
			QueueReader.BeginReceive(CLIENT_A, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_B, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_C, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_D, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_E, WriteToConsole);
			QueueReader.BeginReceive(CLIENT_F, WriteToConsole);
			router.BeginReceive();


			var messages = new List<GateInfo>
			{
				new GateInfo { GateNumber = 1, Info = "Message concerning gate 1" },
				new GateInfo { GateNumber = 2, Info = "Message concerning gate 2" },
				new GateInfo { GateNumber = 3, Info = "Message concerning gate 3" },
				new GateInfo { GateNumber = 5, Info = "Message concerning gate 5" },
				new GateInfo { GateNumber = 8, Info = "Message concerning gate 8" },
				new GateInfo { GateNumber = 13, Info = "Message concerning gate 13" }
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
			message.AttachFormatter(new[] { typeof(GateInfo) });
			var info = (GateInfo)message.Body;
			// message.DestinationQueue.Path har en exception
			Console.WriteLine("Gate: " + info.GateNumber + "->" + "[destination here]" + ": " + info.Info);
		}

		private static void VerifyQueuesExistAndEmpty()
		{
			// Kontroller køerne eksisterer
			ROUTER_QUEUE.VerifyRecreate();
			CLIENT_A.VerifyRecreate();
			CLIENT_B.VerifyRecreate();
			CLIENT_C.VerifyRecreate();
			CLIENT_D.VerifyRecreate();
			CLIENT_E.VerifyRecreate();
			CLIENT_F.VerifyRecreate();

			// Tøm køerne
			ROUTER_QUEUE.Purge();
			CLIENT_A.Purge();
			CLIENT_B.Purge();
			CLIENT_C.Purge();
			CLIENT_D.Purge();
			CLIENT_E.Purge();
			CLIENT_F.Purge();
		}

		private static void CleanUp()
		{
			// Hvorfor udløser dette en exception???????????
			ROUTER_QUEUE.Delete();
			CLIENT_A.Delete();
			CLIENT_B.Delete();
			CLIENT_C.Delete();
			CLIENT_D.Delete();
			CLIENT_E.Delete();
			CLIENT_F.Delete();
		}
	}
}
