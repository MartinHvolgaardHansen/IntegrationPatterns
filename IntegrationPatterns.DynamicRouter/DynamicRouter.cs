using IntegrationPatterns.Common;
using IntegrationPatterns.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.DynamicRouter
{
	class DynamicRouter
	{
		private readonly MessageQueue routerQueue;
		private readonly IDictionary<string, MessageQueue> clientQueues;
		private readonly RoutingRuleRepository repository;

		public DynamicRouter(
			MessageQueue routerQueue, 
			IDictionary<string, MessageQueue> clientQueues, 
			RoutingRuleRepository repository)
		{
			this.routerQueue = routerQueue;
			this.clientQueues = clientQueues;
			this.repository = repository;
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
			var rules = this.repository.Get();
			message.AttachFormatter(new[] { typeof(GateInfo) });
			var gateInfo = (GateInfo)message.Body;

			if (rules.ContainsKey(gateInfo.GateNumber))
			{
				var rule = rules[gateInfo.GateNumber];
				if (IsWithinInterval(rule))
				{
					var clientQueue = this.clientQueues[rule.QueueId];
					clientQueue.Send(message);
				}
			}
		}

		private bool IsWithinInterval(RoutingRule rule)
		{
			var now = DateTime.Now;
			return now > rule.IntervalStart && now < rule.IntervalEnd;
		}
	}
}
