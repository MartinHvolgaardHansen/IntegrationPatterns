using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.DynamicRouter
{
	public class RoutingRule
	{
		public string QueueId { get; set; }
		public int GateNumber { get; set; }
		public DateTime IntervalStart { get; set; }
		public DateTime IntervalEnd { get; set; }
	}
}
